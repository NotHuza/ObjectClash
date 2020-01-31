namespace CR.Servers.CoC.Logic
{
    using System;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using Newtonsoft.Json;

    internal class PlayerBase
    {
        [JsonProperty] internal AchievementProgressSlot AchievementProgress;


        [JsonProperty] internal AchievementSlot Achievements;

        [JsonProperty] internal int AllianceHighId;
        [JsonProperty] internal int AllianceLowId;
        [JsonProperty] internal AllianceUnitSlots AllianceSpells;
        [JsonProperty] internal AllianceUnitSlots AllianceUnits;

        [JsonProperty] internal int CastleLevel = -1;
        [JsonProperty] internal int CastleTotalCapacity;
        [JsonProperty] internal int CastleTotalSpellCapacity;
        [JsonProperty] internal int CastleUsedCapacity;
        [JsonProperty] internal int CastleUsedSpellCapacity;
        [JsonProperty] internal DataSlots HeroHealth;
        [JsonProperty] internal DataSlots HeroModes;

        [JsonProperty] internal DataSlots HeroStates;
        [JsonProperty] internal DataSlots HeroUpgrades;
        internal Level Level;
        [JsonProperty] internal DataSlots NpcLootedElixir;
        [JsonProperty] internal DataSlots NpcLootedGold;

        [JsonProperty] internal NpcMapSlots NpcMapProgress;

        [JsonProperty] internal DataSlots ResourceCaps;
        [JsonProperty] internal ResourceSlots Resources;
        [JsonProperty] internal UnitSlots Spells;
        [JsonProperty] internal DataSlots SpellUpgrades;

        [JsonProperty] internal int TownHallLevel;
        [JsonProperty] internal int TownHallLevel2;

        [JsonProperty] internal UnitSlots UnitPreset1;
        [JsonProperty] internal UnitSlots UnitPreset2;
        [JsonProperty] internal UnitSlots UnitPreset3;

        [JsonProperty] internal UnitSlots Units;
        [JsonProperty] internal UnitSlots Units2;
        [JsonProperty] internal DataSlots UnitUpgrades;
        [JsonProperty] internal VariableSlots Variables;

        [JsonProperty] internal int WallGroupId;

        public PlayerBase()
        {
            this.ResourceCaps = new DataSlots();
            this.Resources = new ResourceSlots();
            this.Units = new UnitSlots();
            this.Units2 = new UnitSlots();
            this.Spells = new UnitSlots();
            this.UnitUpgrades = new DataSlots();
            this.SpellUpgrades = new DataSlots();
            this.HeroUpgrades = new DataSlots();
            this.AllianceUnits = new AllianceUnitSlots();
            this.AllianceSpells = new AllianceUnitSlots();

            this.HeroHealth = new DataSlots();
            this.HeroStates = new DataSlots();
            this.HeroModes = new DataSlots();

            this.NpcMapProgress = new NpcMapSlots();
            this.NpcLootedGold = new DataSlots();
            this.NpcLootedElixir = new DataSlots();

            this.Variables = new VariableSlots();

            this.UnitPreset1 = new UnitSlots();
            this.UnitPreset2 = new UnitSlots();
            this.UnitPreset3 = new UnitSlots();
        }

        internal long AllianceId
        {
            get
            {
                return ((long)this.AllianceHighId << 32) | (uint)this.AllianceLowId;
            }
        }

        internal bool InAlliance
        {
            get
            {
                return this.AllianceId > 0;
            }
        }

        //Testing
        internal bool Village2
        {
            get
            {
                return this.Variables.GetCountByGlobalId(37000012) == 1;
            }
            set
            {
                this.Variables.Set(37000012, Convert.ToInt32(value));
            }
        }

        internal int Map
        {
            get
            {
                return this.Variables.GetCountByGlobalId(37000012);
            }
            set
            {
                this.Variables.Set(37000012, value);
            }
        }

        internal int Gold
        {
            get
            {
                return this.Resources.GetCountByGlobalId(3000001);
            }
            set
            {
                this.Resources.Set(3000001, value);
            }
        }

        internal int Elixir
        {
            get
            {
                return this.Resources.GetCountByGlobalId(3000002);
            }
            set
            {
                this.Resources.Set(3000002, value);
            }
        }

        internal int DarkElixir
        {
            get
            {
                return this.Resources.GetCountByGlobalId(3000003);
            }
            set
            {
                this.Resources.Set(3000003, value);
            }
        }

        internal int Gold2
        {
            get
            {
                return this.Resources.GetCountByGlobalId(3000007);
            }
            set
            {
                this.Resources.Set(3000007, value);
            }
        }

        internal int Elixir2
        {
            get
            {
                return this.Resources.GetCountByGlobalId(3000008);
            }
            set
            {
                this.Resources.Set(3000008, value);
            }
        }

        internal TownhallLevelData TownhallLevelData
        {
            get
            {
                return (TownhallLevelData)CSV.Tables.Get(Gamefile.Townhall_Levels).GetDataWithInstanceID(this.TownHallLevel);
            }
        }

        internal TownhallLevelData TownhallLevel2Data
        {
            get
            {
                return (TownhallLevelData)CSV.Tables.Get(Gamefile.Townhall_Levels).GetDataWithInstanceID(this.TownHallLevel2);
            }
        }

        internal virtual int Checksum
        {
            get
            {
                return this.Resources.Checksum
                       + this.ResourceCaps.Checksum // OutOfSync
                       + this.Units.Checksum
                       + this.Spells.Checksum
                       + this.AllianceUnits.Checksum
                       + this.UnitUpgrades.Checksum
                       + this.SpellUpgrades.Checksum
                       + this.Units2.Checksum
                       + this.TownHallLevel
                       + this.TownHallLevel2;
            }
        }

        internal virtual bool ClientPlayer
        {
            get
            {
                return false;
            }
        }

        internal virtual bool NpcPlayer
        {
            get
            {
                return false;
            }
        }

        internal int GetUnitUpgradeLevel(Data Data)
        {
            return Data.GetDataType() == 4 ? this.UnitUpgrades.GetCountByData(Data) : this.SpellUpgrades.GetCountByData(Data);
        }

        internal void IncreaseUnitUpgradeLevel(Data Data)
        {
            if (Data.GetDataType() == 4)
            {
                this.UnitUpgrades.Add(Data, 1);
            }
            else
            {
                this.SpellUpgrades.Add(Data, 1);
            }
        }

        internal int GetHeroUpgradeLevel(Data Data)
        {
            return this.HeroUpgrades.GetCountByData(Data);
        }

        internal void IncreaseHeroUpgradeLevel(Data Data)
        {
            this.HeroUpgrades.Add(Data, 1);
        }

        internal int GetAvailableResourceStorage(Data Resource)
        {
            return this.ResourceCaps.GetCountByData(Resource) - this.Resources.GetCountByData(Resource);
        }
    }
}