namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class DataSlots : List<Item>
    {
        public DataSlots(int Capacity = 50) : base(Capacity)
        {
            // DataSlots.
        }

        public DataSlots()
        {
        }

        internal int Checksum
        {
            get
            {
                int Checksum = 0;

                for (int i = 0; i < this.Count; i++)
                {
                    Checksum += this[i].Checksum;
                }

                return Checksum;
            }
        }

        internal void Add(Data Data, int Count)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Data.GlobalId, out Current))
            {
                Current.Count += Count;
            }
            else
            {
                base.Add(new Item(Data.GlobalId, Count));
            }
        }

        internal void Add(int Data, int Count)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Data, out Current))
            {
                Current.Count += Count;
            }
            else
            {
                base.Add(new Item(Data, Count));
            }
        }

        internal new void Add(Item Item)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Item.Data, out Current))
            {
                Current.Count += Item.Count;
            }
            else
            {
                base.Add(Item);
            }
        }

        internal Item GetByData(Data Data)
        {
            for (int i = 0; i < Count; i++)
            {
                var item = this[i];
                if (item.Data == Data.GlobalId)
                    return item;
            }

            return null;
        }

        internal Item GetByGlobalId(int Id)
        {
            for (int i = 0; i < Count;i++)
            {
                var item = this[i];
                if (item.Data == Id)
                    return item;
            }

            return null;
        }

        internal int GetCountByData(Data Data)
        {
            return this.Find(T => T.Data == Data.GlobalId)?.Count ?? 0;
        }

        internal int GetCountByGlobalId(int Id)
        {
            return this.Find(T => T.Data == Id)?.Count ?? 0;
        }

        internal void Remove(Data Data, int Count)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Data.GlobalId, out Current))
            {
                Current.Count -= Count;
            }
#if DEBUG
            //else if (Count > 0)
            //Logging.Info(this.GetType(), "Remove() - DataSlot doesn't exist. This should never happen, check existing before remove. (" + Data.GlobalID + ")");
#endif
        }

        internal void Remove(Item Item, int Count)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Item.Data, out Current))
            {
                Current.Count -= Count;

                if (Current.Count < 0)
                {
                    Logging.Info(this.GetType(), "Remove() - Count is inferior at 0. This should never happen, check count before remove.");
                    Current.Count = 0;
                }
            }
#if DEBUG
            //else if (Count > 0)
            //  Logging.Info(this.GetType(), "Remove() - DataSlot doesn't exist. This should never happen, check existing before remove.");
#endif
        }

        internal void Set(int Id, int Count)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Id, out Current))
            {
                Current.Count = Count;
            }
            else
            {
                base.Add(new Item(Id, Count));
            }
        }

        internal void Set(Data Data, int Count)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Data.GlobalId, out Current))
            {
                Current.Count = Count;
            }
            else
            {
                base.Add(new Item(Data.GlobalId, Count));
            }
        }

        internal void Set(Item Item)
        {
            Item Current;
            if (this.TryGet(T => T.Data == Item.Data, out Current))
            {
                Current.Count = Item.Count;
            }
            else
            {
                base.Add(Item);
            }
        }

        internal void Encode(List<byte> Packet)
        {
            Packet.AddInt(this.Count);

            for (int i = 0; i < Count; i++)
                this[i].Encode(Packet);
        }

        internal JArray Save()
        {
            JArray Array = new JArray();

            for (int i = 0; i < Count; i++)
                Array.Add(this[i].Save());

            return Array;
        }
    }
}