namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    using System;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Files.CSV_Reader;

    internal class DecoData : Data
    {
        internal ResourceData BuildResourceData;

        public DecoData(Row Row, DataTable DataTable) : base(Row, DataTable)
        {
        }

        public override string Name { get; set; }
        public string TID { get; set; }
        public string InfoTID { get; set; }
        public string SWF { get; set; }
        public string ExportName { get; set; }
        public string ExportNameConstruction { get; set; }
        public string BuildResource { get; set; }
        public int BuildCost { get; set; }
        public int RequiredExpLevel { get; set; }
        public int MaxCount { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Icon { get; set; }
        public int BaseGfx { get; set; }
        public string ExportNameBase { get; set; }
        public bool IsRed { get; set; }
        public bool NotInShop { get; set; }
        public int VillageType { get; set; }
        public int RedMul { get; set; }
        public int GreenMul { get; set; }
        public int BlueMul { get; set; }
        public int RedAdd { get; set; }
        public int GreenAdd { get; set; }
        public int BlueAdd { get; set; }
        public bool LightsOn { get; set; }

        internal int SellPrice
        {
            get
            {
                int calculation = (int) ((this.BuildCost * (long) 1717986919) >> 32);
                return (calculation >> 2) + (calculation >> 31);
            }
        }

        internal override void Process()
        {
            this.BuildResourceData = (ResourceData) CSV.Tables.Get(Gamefile.Resources).GetData(this.BuildResource);

            if (this.BuildResourceData == null)
            {
                throw new Exception("Buildings.csv: Build Resource is invalid!.");
            }
        }
    }
}