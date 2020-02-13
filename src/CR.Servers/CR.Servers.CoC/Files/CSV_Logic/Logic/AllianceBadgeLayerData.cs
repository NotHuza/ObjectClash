using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.Files.CSV_Reader;

namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class AllianceBadgeLayerData : Data
    {
        public AllianceBadgeLayerData(Row rw, DataTable dt) : base(rw, dt)
        {
        }

        public override string Name { get; set; }

        public string Type { get; set; }

        public string SWF { get; set; }

        public string ExportName { get; set; }

        public int RequiredClanLevel { get; set; }
    }
}
