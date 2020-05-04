namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.Files.CSV_Reader;

    internal class AllianceBadgeLayerData : Data
    {
        public AllianceBadgeLayerData(Row Row, DataTable DataTable) : base(Row, DataTable)
        {
        }

        public override string Name { get; set; }
        public string Type { get; set; }
        public string SWF { get; set; }
        public string ExportName { get; set; }
    }
}