using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.Files.CSV_Reader;

namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class AchievementData : Data
    {
        internal int ACTION_TYPE;
        internal BuildingData buildingData;

        public AchievementData(Row Row, DataTable DataTable) : base(Row, DataTable)
        {
        }

        public override string Name { get; set; }

        public int Level { get; set; }

        public int LevelCount { get; set; }

        public string TID { get; set; }

        public string InfoTID { get; set; }

        public string Action { get; set; }

        public int ActionCount { get; set; }

        public string ActionData { get; set; }

        public int ExpReward { get; set; }

        public int DiamondReward { get; set; }

        public string IconSWF { get; set; }

        public string IconExportName { get; set; }

        public string CompletedTID { get; set; }

        public bool ShowValue { get; set; }

        public string AndroidID { get; set; }

        internal override void Process()
        {
            switch (this.Action)
            {
                case "npc_stars":
                    this.ACTION_TYPE = 0;
                break;

                case "upgrade":
                    this.ACTION_TYPE = 1;
                    this.buildingData = DataTables.GetBuildingByName(this.ActionData);
                break;

                case "victory_points":
                    this.ACTION_TYPE = 2;
                break;
            }
        }
    }
}
