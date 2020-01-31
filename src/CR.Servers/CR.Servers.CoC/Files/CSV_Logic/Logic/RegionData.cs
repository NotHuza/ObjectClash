namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.Files.CSV_Reader;

    internal class RegionData : Data
    {
        public RegionData(Row Row, DataTable DataTable) : base(Row, DataTable)
        {
        }


        public override string Name { get; set; }
        public string TID { get; set; }
        public string DisplayName { get; set; }
        public bool IsCountry { get; set; }
    }
}