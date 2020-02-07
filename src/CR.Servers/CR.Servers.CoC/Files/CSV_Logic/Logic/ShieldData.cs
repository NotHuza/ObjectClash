using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.Files.CSV_Reader;

﻿namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class ShieldData : Data
    {
        public ShieldData(Row rw, DataTable dt) : base(rw, dt)
        {
        }

        public override string Name { get; set; }

        public string TID { get; set; }

        public string InfoTID { get; set; }

        public int TimeH { get; set; }

        public int GuardTimeH { get; set; }

        public int Diamonds { get; set; }

        public string IconSWF { get; set; }

        public string IconExportName { get; set; }

        public int CooldownS { get; set; }

        public int LockedAboveScore { get; set; }
    }
}
