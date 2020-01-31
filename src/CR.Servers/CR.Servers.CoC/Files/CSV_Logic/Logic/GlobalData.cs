namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.Files.CSV_Reader;

    internal class GlobalData : Data
    {
        public GlobalData(Row Row, DataTable DataTable) : base(Row, DataTable)
        {
        }

        public override string Name { get; set; }
        public int NumberValue { get; set; }
        public bool BooleanValue { get; set; }
        public string TextValue { get; set; }
        public int[] NumberArray { get; set; }
        public int[] AltNumberArray { get; set; }
        public string StringArray { get; set; }
    }
}