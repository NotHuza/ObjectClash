namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class Item
    {
        private Data _data;
        [JsonProperty("cnt")] internal int Count;
        [JsonProperty("id")] internal int Data;

        public Item()
        {
        }

        public Item(int Data, int Count)
        {
            this.Data = Data;
            this.Count = Count;
        }

        internal Data ItemData
        {
            get
            {
                return this._data ?? (this._data = CSV.Tables.GetWithGlobalId(this.Data));
            }
            set
            {
                this._data = value;
            }
        }

        internal virtual int Checksum
        {
            get
            {
                return this.Data + this.Count;
            }
        }

        internal virtual void Decode(Reader Reader)
        {
            this.Data = Reader.ReadInt32();
            this.Count = Reader.ReadInt32();
            this.ItemData = CSV.Tables.GetWithGlobalId(this.Data);
        }

        internal virtual void Encode(List<byte> Packet)
        {
            Packet.AddInt(this.Data);
            Packet.AddInt(this.Count);
        }

        internal virtual void Load(JToken Token)
        {
            if (JsonHelper.GetJsonNumber(Token, "id", out this.Data))
            {
                this.ItemData = CSV.Tables.GetWithGlobalId(this.Data);
            }

            JsonHelper.GetJsonNumber(Token, "cnt", out this.Count);
        }

        internal virtual JObject Save()
        {
            JObject Json = new JObject {{"id", this.Data}, {"cnt", this.Count}};
            return Json;
        }

        public static Item operator +(Item Item, Item Item2)
        {
            return Item.Data == Item2.Data ? new Item(Item.Data, Item.Count + Item2.Count) : null;
        }

        public static Item operator -(Item Item, Item Item2)
        {
            return Item.Data == Item2.Data ? new Item(Item.Data, Item.Count + Item2.Count) : null;
        }

        public override bool Equals(object obj)
        {
            if (obj is Item)
                return ((Item)obj).Data == this.Data;

            return false;
        }
    }
}