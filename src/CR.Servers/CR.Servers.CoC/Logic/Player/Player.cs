namespace CR.Servers.CoC.Logic
{
    using System;
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using CR.Servers.Logic.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Packets;

    internal class Player : PlayerBase
    {
#if COMMAND_DEBUG
        internal CommandDebug Debug;
#endif

        internal Alliance Alliance;
        internal Member AllianceMember;
        [JsonProperty] internal DateTime BanTime = DateTime.UtcNow;

        internal long BattleIdV2;

        [JsonProperty] internal int ChangeNameCount;
        [JsonProperty] internal int ClanWarPreference = 1;
        [JsonProperty] internal DateTime Created = DateTime.UtcNow;

        [JsonProperty] internal int Diamonds;

        [JsonProperty] internal int Donation;
        [JsonProperty] internal int DonationReceived;
        [JsonProperty] internal int DuelScore;

        [JsonProperty] internal int ExpLevel = 1;
        [JsonProperty] internal int ExpPoints;

        [JsonProperty] internal FacebookApi Facebook;
        [JsonProperty] internal int FreeDiamonds;
        [JsonProperty] internal Friends Friends;
        [JsonProperty] internal int Games;

        [JsonProperty] internal int HighID;
        [JsonProperty] internal Inbox Inbox;

        [JsonProperty] internal int LastSeasonMonth;
        [JsonProperty] internal int LastSeasonRank;
        [JsonProperty] internal int LastSeasonScore;

        [JsonProperty] internal int LastSeasonYear;
        /*[JsonProperty] internal Google Google;
        [JsonProperty] internal Gamecenter Gamecenter;*/

        [JsonProperty] internal DateTime LastTick = DateTime.UtcNow;
        [JsonProperty] internal int League;

        [JsonProperty] internal bool Locked;

        [JsonProperty] internal int LogSeed;
        [JsonProperty] internal int Loses;
        [JsonProperty] internal int LowID;

        [JsonProperty] internal ModSlot ModSlot;

        [JsonProperty] internal string Name = string.Empty;
        [JsonProperty] internal bool NameSetByUser;

        [JsonProperty] internal int ObstacleCleaned;

        [JsonProperty] internal int OldSeasonMonth;
        [JsonProperty] internal int OldSeasonRank;
        [JsonProperty] internal int OldSeasonScore;
        [JsonProperty] internal int OldSeasonYear;
        [JsonProperty] internal string Password;

        [JsonProperty] internal Rank Rank = Rank.Player;

        [JsonProperty] internal bool RedPackageState;
        [JsonProperty] internal int Score;

        [JsonProperty] internal string Token;

        [JsonProperty] internal List<int> Tutorials = new List<int>();
        [JsonProperty] internal DateTime Update = DateTime.UtcNow;

        [JsonProperty] internal int Wins;

        internal Player()
        {
#if COMMAND_DEBUG
            this.Debug = new CommandDebug(this);
#endif
            this.Inbox = new Inbox(this);
            this.Friends = new Friends(this);
            this.ModSlot = new ModSlot();
            this.Achievements = new AchievementSlot();
            this.AchievementProgress = new AchievementProgressSlot(this);

            this.Facebook = new FacebookApi(this);
            /*
            this.Missions = new List<Data>(30);
            this.Logs = new List<AvatarStreamEntry>(50);

            this.Facebook = new Facebook(this);
            this.Google = new Google(this);
            this.Gamecenter = new Gamecenter(this);
            */
        }

        internal Player(Level Level, int HighID, int LowID) : this()
        {
            this.Level = Level;
            this.HighID = HighID;
            this.LowID = LowID;

            if (Extension.ParseConfigBoolean("Game:StartingResources:FetchFromCSV"))
            {
                this.Diamonds = Globals.StartingDiamonds;
                this.FreeDiamonds = Globals.StartingDiamonds;
            }
            else
            {
                this.Diamonds = Extension.ParseConfigInt("Game:StartingResources:Diamonds");
                this.FreeDiamonds = Extension.ParseConfigInt("Game:StartingResources:Diamonds");
            }

            this.Resources.Initialize();
            this.Variables.Initialize();
            this.ModSlot.Initialize();

            this.Units2.Add(CSV.Village2StartUnit, CSV.Village2StartUnit.UnitsInCamp[0]);
        }

        internal bool Connected
        {
            get
            {
                if (this.Level?.GameMode != null)
                {
                    return this.Level.GameMode.Connected;
                }

                return false;
            }
        }

        internal long UserId
        {
            get
            {
                return ((long) this.HighID << 32) | (uint) this.LowID;
            }
        }

        internal bool Banned
        {
            get
            {
                return this.BanTime > DateTime.UtcNow;
            }
        }

        internal override int Checksum
        {
            get
            {
                return this.ExpPoints
                       + this.ExpLevel
                       + this.Diamonds
                       + this.FreeDiamonds
                       + this.Score
                       + this.DuelScore
                       + (this.AllianceLowId > 0 ? 13 : 0)
                       + base.Checksum;
            }
        }

        internal void Process()
        {
            //this.Friends.VerifyFriend(); // Causes StackOverFlowException at the moment.
            this.VerifyAlliance();
        }

        internal void AddDiamonds(int Diamonds)
        {
            this.Diamonds += Diamonds;
            this.FreeDiamonds += Diamonds;
        }

        internal void AddExperience(int ExpPoints)
        {
            ExperienceLevelData ExperienceData = (ExperienceLevelData) CSV.Tables.Get(Gamefile.Experience_Levels).GetDataWithID(this.ExpLevel - 1);

            this.ExpPoints += ExpPoints;

            if (this.ExpPoints >= ExperienceData?.ExpPoints)
            {
                this.ExpLevel++;
                this.ExpPoints -= ExperienceData.ExpPoints;
            }
        }

        internal void AddScore(int Trophies)
        {
            this.Score += Trophies;

            LeagueData LeagueData = (LeagueData) CSV.Tables.Get(Gamefile.Leagues).GetDataWithInstanceID(this.League);

            if (LeagueData?.PlacementLimitHigh > this.Score)
            {
                for (int i = LeagueData.GetId(); i < CSV.Tables.Get(Gamefile.Leagues).Datas.Count; i++)
                {
                    LeagueData Data = (LeagueData) CSV.Tables.Get(Gamefile.Leagues).Datas[i];

                    if (this.Score >= Data.PlacementLimitLow)
                    {
                        this.League = i;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        internal void VerifyAlliance()
        {
            if (this.Alliance == null)
            {
                Alliance Clan = Core.Resources.Clans.Get(this.AllianceHighId, this.AllianceLowId);
                if (Clan != null)
                {
                    Member Member = Clan.Members.Get(this.UserId);
                    if (Member != null)
                    {
                        this.SetAlliance(Clan, Member);
                    }
                    else
                    {
                        this.AllianceHighId = 0;
                        this.AllianceLowId = 0;
                    }
                }
                else
                {
                    this.AllianceHighId = 0;
                    this.AllianceLowId = 0;
                }
            }
        }

        internal void SetAlliance(Alliance Alliance, Member Member)
        {
            this.Alliance = Alliance;
            this.AllianceMember = Member;
        }

        internal void Encode(List<byte> _Packet)
        {
            _Packet.AddLong(this.UserId);
            _Packet.AddLong(this.UserId);

            if (this.InAlliance)
            {
                _Packet.AddBool(true);
                _Packet.AddLong(this.AllianceId);
                _Packet.AddString(this.Alliance.Header.Name);

                _Packet.AddInt(this.Alliance.Header.Badge);
                _Packet.AddInt((int) this.AllianceMember.Role);
                _Packet.AddInt(this.Alliance.Header.ExpLevel);

                _Packet.AddBool(false); //War Id or league id?
            }
            else
            {
                _Packet.AddBool(false);
            }

            _Packet.AddInt(0); //Legendary_Trophies

            _Packet.AddInt(this.LastSeasonYear > 0 ? 1 : 0);

            _Packet.AddInt(this.LastSeasonMonth);
            _Packet.AddInt(this.LastSeasonYear);
            _Packet.AddInt(this.LastSeasonRank);
            _Packet.AddInt(this.LastSeasonScore);

            _Packet.AddInt(this.OldSeasonYear > 0 ? 1 : 0);

            _Packet.AddInt(this.OldSeasonMonth);
            _Packet.AddInt(this.OldSeasonYear);
            _Packet.AddInt(this.OldSeasonRank);
            _Packet.AddInt(this.OldSeasonScore);

            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0);

            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0);
            _Packet.AddInt(0); //Builder base Versus Battle Win
            _Packet.AddInt(0); //5
            _Packet.AddInt(0); //8

            _Packet.AddInt(this.League);
            _Packet.AddInt(this.CastleLevel);
            _Packet.AddInt(this.CastleTotalCapacity);
            _Packet.AddInt(this.CastleUsedCapacity);
            _Packet.AddInt(this.CastleTotalSpellCapacity);
            _Packet.AddInt(this.CastleUsedSpellCapacity);

            _Packet.AddInt(this.TownHallLevel);
            _Packet.AddInt(this.TownHallLevel2);


            _Packet.AddString(this.Name);
            _Packet.AddString(this.Facebook.Identifier == string.Empty ? null : this.Facebook.Identifier);

            _Packet.AddInt(this.ExpLevel);
            _Packet.AddInt(this.ExpPoints);
            _Packet.AddInt(this.Diamonds);
            _Packet.AddInt(this.FreeDiamonds);

            _Packet.AddInt(0); // 1200
            _Packet.AddInt(0); // 60

            _Packet.AddInt(this.Score);
            _Packet.AddInt(this.DuelScore);

            _Packet.AddInt(this.Wins);
            _Packet.AddInt(this.Loses);
            _Packet.AddInt(0); // Def Wins
            _Packet.AddInt(0); // Def Loses

            _Packet.AddInt(0); //WarGold
            _Packet.AddInt(0); //WarElixir
            _Packet.AddInt(0); //WarDarkElixir

            _Packet.AddInt(0);

            _Packet.AddBool(true);
            _Packet.AddInt(220);
            _Packet.AddInt(1828055880);

            _Packet.AddBool(this.NameSetByUser); //Name changed count

            _Packet.AddInt(this.ChangeNameCount); //Name Changed

            _Packet.AddInt(6900); //6900
            _Packet.AddInt(0);
            _Packet.AddInt(this.ClanWarPreference);

            _Packet.AddInt(0);
            _Packet.AddInt(0); // Total Attack with shield

            _Packet.AddBool(false); //0

            this.ResourceCaps.Encode(_Packet);
            this.Resources.Encode(_Packet);
            this.Units.Encode(_Packet);
            this.Spells.Encode(_Packet);

            this.UnitUpgrades.Encode(_Packet);
            this.SpellUpgrades.Encode(_Packet);
            this.HeroUpgrades.Encode(_Packet);

            this.HeroHealth.Encode(_Packet);
            this.HeroStates.Encode(_Packet);

            _Packet.AddInt(this.AllianceUnits.Count);

            foreach (UnitItem Unit in this.AllianceUnits)
            {
                _Packet.AddInt(Unit.Data);
                _Packet.AddInt(Unit.Count);
                _Packet.AddInt(Unit.Level);
            }
             
            _Packet.AddInt(this.Tutorials.Count);
            foreach (int Tutorial in this.Tutorials)
            {
                _Packet.AddInt(Tutorial);
            }

            this.Achievements.Encode(_Packet);
            this.AchievementProgress.Encode(_Packet);

            this.NpcMapProgress.Encode(_Packet);
            this.NpcLootedGold.Encode(_Packet);
            this.NpcLootedElixir.Encode(_Packet);

            _Packet.AddInt(0);
            _Packet.AddInt(0);
            this.HeroModes.Encode(_Packet);

            this.Variables.Encode(_Packet);

            this.UnitPreset1.Encode(_Packet);
            this.UnitPreset2.Encode(_Packet);
            this.UnitPreset3.Encode(_Packet);

            _Packet.AddInt(0); // PreviousArmySize
            _Packet.AddInt(0); // UnitCounterForEvent
            this.Units2.Encode(_Packet);
            _Packet.AddInt(0);
            _Packet.AddInt(0);
            //_Packet.AddInt(0);
        }
        
        internal JObject Save()
        {
            JObject Json = new JObject
            {
                {"avatar_id_high", this.HighID},
                {"avatar_id_low", this.LowID},
                {"name", this.Name},
                {"alliance_name", this.InAlliance ? this.Alliance.Header.Name : string.Empty},
                {"xp_level", this.ExpLevel}
            };

            if (this.InAlliance)
            {
                Json.Add("alliance_id_high", this.AllianceHighId);
                Json.Add("alliance_id_low", this.AllianceLowId);
                Json.Add("badge_id", this.Alliance.Header.Badge);
                Json.Add("alliance_exp_level", this.Alliance.Header.ExpLevel);
            }

            Json.Add("alliance_unit_visit_capacity", 0);
            Json.Add("alliance_unit_spell_visit_capacity", 0);

            Json.Add("league_type", this.League);

            Json.Add("units", this.Units.Save());
            Json.Add("spells", this.Spells.Save());
            Json.Add("unit_upgrades", this.UnitUpgrades.Save());
            Json.Add("spell_upgrades", this.SpellUpgrades.Save());

            Json.Add("resources", this.Resources.Save());

            Json.Add("alliance_units", this.AllianceUnits.Save());

            Json.Add("hero_states", this.HeroStates.Save());
            Json.Add("hero_health", this.HeroHealth.Save());
            Json.Add("hero_upgrade", this.HeroUpgrades.Save());
            Json.Add("hero_modes", this.HeroModes.Save());

            Json.Add("variables", this.Variables.Save());
            Json.Add("units2", this.Units2.Save());

            Json.Add("castle_lvl", this.CastleLevel);

            Json.Add("castle_total", this.CastleTotalCapacity);
            Json.Add("castle_used", this.CastleUsedCapacity);
            Json.Add("castle_total_sp", this.CastleTotalSpellCapacity);
            Json.Add("castle_used_sp", this.CastleUsedSpellCapacity);

            Json.Add("town_hall_lvl", this.TownHallLevel);
            Json.Add("th_v2_lvl", this.TownHallLevel2);

            Json.Add("score", this.Score);
            Json.Add("duel_score", this.DuelScore);

            if (this.RedPackageState)
            {
                Json.Add("red_package_state", this.RedPackageState);
            }

            return Json;
        }

        internal void Battle(JObject Json)
        {
            Json.Add("avatar_id_high", this.HighID);
            Json.Add("avatar_id_low", this.LowID);
            Json.Add("name", this.Name);
            Json.Add("alliance_name", this.InAlliance ? this.Alliance.Header.Name : string.Empty);
            Json.Add("xp_level", this.ExpLevel);

            if (this.InAlliance)
            {
                Json.Add("alliance_id_high", this.AllianceHighId);
                Json.Add("alliance_id_low", this.AllianceLowId);
                Json.Add("badge_id", this.Alliance.Header.Badge);
                Json.Add("alliance_exp_level", this.Alliance.Header.ExpLevel);
            }

            Json.Add("alliance_unit_visit_capacity", 0);
            Json.Add("alliance_unit_spell_visit_capacity", 0);

            Json.Add("league_type", this.League);

            Json.Add("resources", this.Resources.Save());

            Json.Add("alliance_units", this.AllianceUnits.Save());

            Json.Add("hero_states", this.HeroStates.Save());
            Json.Add("hero_health", this.HeroHealth.Save());
            Json.Add("hero_upgrade", this.HeroUpgrades.Save());
            Json.Add("hero_modes", this.HeroModes.Save());

            Json.Add("variables", this.Variables.Save());

            Json.Add("castle_lvl", this.CastleLevel);

            Json.Add("castle_total", this.CastleTotalCapacity);
            Json.Add("castle_used", this.CastleUsedCapacity);
            Json.Add("castle_total_sp", this.CastleTotalSpellCapacity);
            Json.Add("castle_used_sp", this.CastleUsedSpellCapacity);

            Json.Add("town_hall_lvl", this.TownHallLevel);
            Json.Add("th_v2_lvl", this.TownHallLevel2);

            Json.Add("score", this.Score);
            Json.Add("duel_score", this.DuelScore);

            if (this.RedPackageState)
            {
                Json.Add("red_package_state", this.RedPackageState);
            }
        }

        internal bool HasEnoughDiamonds(int Count)
        {
            return this.Diamonds >= Count;
        }

        internal void UseDiamonds(int Count)
        {
            this.Diamonds -= Count;
            this.FreeDiamonds -= Count;

            if (this.FreeDiamonds < 0)
            {
                this.FreeDiamonds = 0;
            }
        }

        internal Player Copy()
        {
            Player Copy = this.MemberwiseClone() as Player;

            Copy.Units = new UnitSlots();
            Copy.Spells = new UnitSlots();
            Copy.HeroHealth = new DataSlots();

            return Copy;
        }

        internal void SetAllianceCastleLevel(int level)
        {
            this.CastleLevel = level;

            if (this.CastleLevel == -1)
            {
                this.CastleTotalCapacity = 0;
                this.CastleTotalSpellCapacity = 0;
            }
            else
            {
                BuildingData allianceCastleData = CSV.AllianceCastleData;

                this.CastleTotalCapacity = allianceCastleData.HousingSpace[level];
                this.CastleTotalSpellCapacity = allianceCastleData.HousingSpaceAlt[level];
            }
        }

        public override string ToString()
        {
            return this.HighID + "-" + this.LowID;
        }
    }
}
