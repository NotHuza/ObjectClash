namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Files.CSV_Reader;

    internal class VillageObjectData : Data
    {
        internal ResourceData BuildResourceData;

        public VillageObjectData(Row Row, DataTable DataTable) : base(Row, DataTable)
        {
        }

        public override string Name { get; set; }
        public string TID { get; set; }
        public string InfoTID { get; set; }
        public string[] SWF { get; set; }
        public string[] ExportName { get; set; }
        public int[] TileX100 { get; set; }
        public int[] TileY100 { get; set; }
        public int RequiredTH { get; set; }
        public int[] BuildTimeD { get; set; }
        public int[] BuildTimeH { get; set; }
        public int[] BuildTimeM { get; set; }
        public int[] BuildTimeS { get; set; }
        public bool RequiresBuilder { get; set; }
        public string BuildResource { get; set; }
        public int[] BuildCost { get; set; }
        public int[] TownHallLevel { get; set; }
        public string PickUpEffect { get; set; }
        public string Animations { get; set; }
        public int AnimX { get; set; }
        public int AnimY { get; set; }
        public int AnimID { get; set; }
        public int AnimDir { get; set; }
        public int AnimVisibilityOdds { get; set; }
        public bool HasInfoScreen { get; set; }
        public int VillageType { get; set; }
        public int UnitHousing { get; set; }

        internal int MaxLevel
        {
            get
            {
                return this.BuildCost.Length - 1;
            }
        }

        internal override void Process()
        {
            this.BuildResourceData = (ResourceData) CSV.Tables.Get(Gamefile.Resources).GetData(this.BuildResource);
        }

        internal int GetBuildTime(int Level)
        {
            return this.BuildTimeD[Level] * 86400 + this.BuildTimeH[Level] * 3600 + this.BuildTimeM[Level] * 60 + this.BuildTimeS[Level];
        }
    }
}