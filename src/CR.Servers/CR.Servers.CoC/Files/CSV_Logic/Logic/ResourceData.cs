using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.Files.CSV_Reader;

﻿namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class ResourceData : Data
    {
        internal ResourceData warRefResourceD;

        internal EffectData collectEffectD;
        internal EffectData stealEffectD;
        internal EffectData stealEffectMidD;
        internal EffectData stealEffectBigD;

        public ResourceData(Row rw, DataTable dt) : base(rw, dt)
        {
        }

        public override string Name { get; set; }

        public string TID { get; set; }

        public string SWF { get; set; }

        public string CollectEffect { get; set; }

        public string ResourceIconExportName { get; set; }

        public string StealEffect { get; set; }

        public int StealLimitMid { get; set; }

        public string StealEffectMid { get; set; }

        public int StealLimitBig { get; set; }

        public string StealEffectBig { get; set; }

        public bool PremiumCurrency { get; set; }

        public string HudInstanceName { get; set; }

        public string CapFullTID { get; set; }

        public int TextRed { get; set; }

        public int TextGreen { get; set; }

        public int TextBlue { get; set; }

        public string WarRefResource { get; set; }

        public string BundleIconExportName { get; set; }

        public int VillageType { get; set; }

        internal override void Process()
        {
            this.warRefResourceD = DataTables.GetResourceByName(this.WarRefResource);
            this.collectEffectD = DataTables.GetEffectByName(this.﻿CollectEffect);
            this.stealEffectD = DataTables.GetEffectByName(this.StealEffect);
            this.stealEffectMidD = DataTables.GetEffectByName(this.StealEffectMid);
            this.stealEffectBigD = DataTables.GetEffectByName(this.StealEffectBig);
        }
    }
}
