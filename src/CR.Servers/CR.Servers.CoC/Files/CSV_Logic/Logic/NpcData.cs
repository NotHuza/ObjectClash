using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.Files.CSV_Reader;

﻿namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class NpcData : Data
    {
        internal CharacterData UnitTypeD;

        public NpcData(Row rw, DataTable dt) : base(rw, dt)
        {
            // NpcData.
        }

        public override string Name { get; set; }

        public string MapInstanceName { get; set; }

        public string[] MapDependencies { get; set; }

        public string TID { get; set; }

        public int ExpLevel { get; set; }

        public string UnitType { get; set; }

        public int UnitCount { get; set; }

        public string LevelFile { get; set; }

        public int Gold { get; set; }

        public int Elixir { get; set; }

        public bool AlwaysUnlocked { get; set; }

        public string PlayerName { get; set; }

        public string AllianceName { get; set; }

        public int AllianceBadge { get; set; }

        public bool[] SinglePlayer { get; set; }

        internal override void Process()
        {
          this.unitTypeD = DataTables.GetCharacterByName(this.UnitType);
        }
    }
}
