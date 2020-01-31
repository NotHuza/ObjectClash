namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class AllianceUnitSlots : List<UnitItem>
    {
        public AllianceUnitSlots(int Capacity = 50) : base(Capacity)
        {
            // AllianceUnitSlots.
        }

        public AllianceUnitSlots()
        {
            // AllianceUnitSlots.
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

        internal void Add(Data Data, int Count, int Level)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Data.GlobalId && T.Level == Level, out Current))
            {
                Current.Count += Count;
            }
            else
            {
                base.Add(new UnitItem(Data.GlobalId, Count, Level));
            }
        }

        internal void Add(int Data, int Count, int Level)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Data && T.Level == Level, out Current))
            {
                Current.Count += Count;
            }
            else
            {
                base.Add(new UnitItem(Data, Count, Level));
            }
        }

        internal new void Add(UnitItem Item)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Item.Data && T.Level == Item.Level, out Current))
            {
                Current.Count += Item.Count;
            }
            else
            {
                base.Add(Item);
            }
        }

        internal void Add(UnitItem Item, long DonatorId)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Item.Data && T.Level == Item.Level && T.DonatorId == DonatorId, out  Current))
            {
                Current.Count += Item.Count;
            }
            else
            {
                base.Add(Item);
            }
        }

        internal Item GetByData(Data Data, int Level)
        {
            return this.Find(T => T.Data == Data.GlobalId && T.Level == Level);
        }

        internal Item GetByGlobalId(int Id, int Level)
        {
            return this.Find(T => T.Data == Id && T.Level == Level);
        }

        internal int GetCountByData(Data Data, int Level)
        {
            return this.Find(T => T.Data == Data.GlobalId && T.Level == Level)?.Count ?? 0;
        }

        internal int GetCountByGlobalId(int Id, int Level)
        {
            return this.Find(T => T.Data == Id && T.Level == Level)?.Count ?? 0;
        }

        internal void Remove(Data Data, int Count, int Level)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Data.GlobalId && T.Level == Level, out Current))
            {
                Current.Count -= Count;
            }
        }

        internal void Remove(Item Item, int Count, int Level)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Item.Data && T.Level == Level, out Current))
            {
                Current.Count -= Count;

                if (Current.Count < 0)
                {
                    //Logging.Info(this.GetType(), "Remove() - Count is inferior at 0. This should never happen, check count before remove.");
                    Current.Count = 0;
                }
            }

            //else
            //Logging.Info(this.GetType(), "Remove() - DataSlot doesn't exist. This should never happen, check existing before remove.");
        }

        internal void Set(int Id, int Count, int Level)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Id && T.Level == Level, out Current))
            {
                Current.Count = Count;
            }
            else
            {
                this.Add(new UnitItem(Id, Count, Level));
            }
        }

        internal void Set(Data Data, int Count, int Level)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Data.GlobalId && T.Level == Level, out Current))
            {
                Current.Count = Count;
            }
            else
            {
                this.Add(new UnitItem(Data.GlobalId, Count, Level));
            }
        }

        internal void Set(UnitItem Item)
        {
            UnitItem Current;
            if (this.TryGet(T => T.Data == Item.Data && T.Level == Item.Level, out Current))
            {
                Current.Count = Item.Count;
            }
            else
            {
                this.Add(Item);
            }
        }

        internal void Encode(List<byte> Packet)
        {
            Packet.AddInt(this.Count);

            for (int i = 0; i < this.Count; i++)
            {
                this[i].Encode(Packet);
            }
        }

        internal JArray Save()
        {
            JArray Array = new JArray();

            for (int i = 0; i < this.Count; i++)
            {
                Array.Add(this[i].Save());
            }

            return Array;
        }

        internal void Load(JToken Token)
        {
            foreach (JToken Unit in Token)
            {
                UnitItem Item = new UnitItem();
                Item.Load(Unit);
                this.Add(Item);
            }
        }

        internal AllianceUnitSlots Copy()
        {
            AllianceUnitSlots Copy = this.MemberwiseClone() as AllianceUnitSlots;

            if (Copy != null)
            {
                foreach (UnitItem Item in Copy)
                {
                    Item.DonatorId = 0;
                }
            }

            return Copy;
        }
    }
}