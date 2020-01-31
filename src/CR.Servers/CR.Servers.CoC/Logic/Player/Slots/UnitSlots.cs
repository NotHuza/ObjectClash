namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;

    internal class UnitSlots : DataSlots
    {
        internal UnitSlots(int Capacity) : base(Capacity)
        {
            // UnitSlots.
        }

        internal UnitSlots()
        {
            // UnitSlots.
        }

        internal int GetUnitsTotal()
        {
            int Total = 0;

            for (int i = 0; i < this.Count; i++)
            {
                Total += this[i].Count;
            }

            return Total;
        }

        internal int GetUnitsTotalCapacity()
        {
            int Total = 0;

            for (int i = 0; i < this.Count; i++)
            {
                Data Data = CSV.Tables.GetWithGlobalId(this[i].Data);
                Total += this[i].Count * (Data.GetDataType() == 4 ? ((CharacterData)Data).HousingSpace : ((SpellData)Data).HousingSpace);
            }

            return Total;
        }
    }
}