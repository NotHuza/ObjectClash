namespace CR.Servers.CoC.Logic.Battles
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using Newtonsoft.Json.Linq;
    using CR.Servers.Titan;

    internal class BattleLog
    {
        internal int VillageType;

        internal Battle Battle;

        internal LogicArrayList<JArray> Loots;
        internal LogicArrayList<JArray> AvailableLoots;
        internal LogicArrayList<JArray> Units;
        internal LogicArrayList<JArray> CastleUnits;
        internal LogicArrayList<JArray> Spells;
        internal LogicArrayList<JArray> Levels;

        // Stats

        internal int AllianceBadge = -1;
        internal int AllianceBadge2 = -1;
        internal int AllianceExp = -1;
        internal int AllianceExp2 = -1;
        internal string AllianceName = string.Empty;
        internal string AttackerName = string.Empty;
        internal string DefenderName = string.Empty;
        internal bool AllianceUsed;
        internal int ArmyDeploymentPercentage;
        internal int AttackerScore;
        internal int AttackerStars;
        internal bool BattleEnded;
        internal int BattleTime;
        internal int DefenderScore;
        internal int DeployedHousingSpace;
        internal int DestructionPercentage;
        internal int[] HomeId = { 0, 0 };
        internal int[] AllianceId = { 0, 0 };
        internal int OriginalAttackerScore;
        internal int OriginalDefenderScore;
        internal bool TownHallDestroyed;

        internal BattleLog(Battle battle)
        {
            this.Battle = battle;

            this.Loots = new LogicArrayList<JArray>(8);
            this.AvailableLoots = new LogicArrayList<JArray>(8);
            this.Units = new LogicArrayList<JArray>(8);
            this.CastleUnits = new LogicArrayList<JArray>(8);
            this.Spells = new LogicArrayList<JArray>(8);
            this.Levels = new LogicArrayList<JArray>(8);

            this.TownHallDestroyed = true;
            this.DestructionPercentage = 100;
            this.OriginalAttackerScore = battle.Attacker.Player.Score;
            this.OriginalDefenderScore = battle.Defender.Player.Score;
            this.DefenderScore = battle.Defender.Player.Score - 30;
            this.AttackerScore = battle.Attacker.Player.Score + 30;
        }

        internal JObject Save()
        {
            JObject json = new JObject();
            JArray arrayLoot = new JArray();
            JArray arrayAvailablLoot = new JArray();
            JArray arrayUnits = new JArray();
            JArray arrayCastleUnits = new JArray();
            JArray arraySpells = new JArray();
            JArray arrayLevels = new JArray();

            for (int i = 0; i < this.Loots.Count; i++)
            {
                arrayLoot.Add(this.Loots[i]);
            }

            for (int i = 0; i < this.AvailableLoots.Count; i++)
            {
                arrayAvailablLoot.Add(this.AvailableLoots[i]);
            }

            for (int i = 0; i < this.Units.Count; i++)
            {
                arrayUnits.Add(this.Units[i]);
            }

            for (int i = 0; i < this.CastleUnits.Count; i++)
            {
                arrayCastleUnits.Add(this.CastleUnits[i]);
            }

            for (int i = 0; i < this.Spells.Count; i++)
            {
                arraySpells.Add(this.Spells[i]);
            }

            for (int i = 0; i < this.Levels.Count; i++)
            {
                arrayLevels.Add(this.Levels[i]);
            }

            json.Add("loot", arrayLoot);
            json.Add("availableLoot", arrayAvailablLoot);
            json.Add("units", arrayUnits);
            json.Add("cc_units", arrayCastleUnits);
            json.Add("spells", arraySpells);
            json.Add("levels", arrayLevels);

            JObject stats = new JObject();

            stats.Add("townhallDestroyed", this.TownHallDestroyed);
            stats.Add("battleEnded", this.BattleEnded);
            stats.Add("allianceUsed", this.AllianceUsed);
            stats.Add("destructionPercentage", this.DestructionPercentage);
            stats.Add("battleTime", this.BattleTime);
            stats.Add("originalAttackerScore", this.OriginalAttackerScore);
            stats.Add("attackerScore", this.AttackerScore);
            stats.Add("originalDefenderScore", this.OriginalDefenderScore);
            stats.Add("defenderScore", this.DefenderScore);
            stats.Add("allianceName", this.AllianceName);
            stats.Add("attackerStars", this.AttackerStars);
            stats.Add("attackerName", this.AttackerName);
            stats.Add("defenderName", this.DefenderName);
            stats.Add("lootMultiplierByTownHallDiff", 0);
            stats.Add("homeID", new JArray
            {
                this.HomeId[0],
                this.HomeId[1]
            });
            stats.Add("allianceBadge", this.AllianceBadge);
            stats.Add("allianceBadge2", this.AllianceBadge2);
            stats.Add("allianceID", new JArray
            {
                this.AllianceId[0],
                this.AllianceId[1]
            });
            stats.Add("deployedHousingSpace", 200);
            stats.Add("armyDeploymentPercentage", 100);
            stats.Add("allianceExp", this.AllianceExp);
            stats.Add("allianceExp2", this.AllianceExp2);

            json.Add("stats", stats);

            return json;
        }

        internal JArray DataSlotArrayToJSONArray(Item item)
        {
            return new JArray
            {
                item.Data,
                item.Count
            };
        }

        internal JArray UnitSlotArrayToJSONArray(UnitItem item)
        {
            return new JArray
            {
                item.Data,
                item.Count
            };
        }

        internal void IncrementUnit(CharacterData data)
        {
            int index = -1;

            for (int i = 0; i < this.Units.Count; i++)
            {
                JArray array = this.Units[i];

                if ((int) array[0] == data.GlobalId)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this.Units[index][1] = (int) this.Units[index][1] + 1;
            }
            else
            {
                this.Units.Add(new JArray
                {
                    data.GlobalId,
                    1
                });
                this.SetLevel(data.GlobalId, this.Battle.Attacker.Player.UnitUpgrades.GetCountByData(data));
            }
        }

        internal void IncrementSpell(SpellData data)
        {
            int index = -1;

            for (int i = 0; i < this.Spells.Count; i++)
            {
                JArray array = this.Spells[i];

                if ((int) array[0] == data.GlobalId)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this.Spells[index][1] = (int) this.Spells[index][1] + 1;
            }
            else
            {
                this.Spells.Add(new JArray
                {
                    data.GlobalId,
                    1
                });
                this.SetLevel(data.GlobalId, this.Battle.Attacker.Player.SpellUpgrades.GetCountByData(data));
            }
        }

        internal void SetLevel(int dataId, int level)
        {
            int index = -1;

            for (int i = 0; i < this.Levels.Count; i++)
            {
                JArray array = this.Levels[i];

                if ((int) array[0] == dataId)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                this.Levels.Add(new JArray
                {
                    dataId,
                    level
                });
            }
        }

        internal void HeroDeployed(HeroData data)
        {
            int index = -1;

            for (int i = 0; i < this.Units.Count; i++)
            {
                JArray array = this.Units[i];

                if ((int) array[0] == data.GlobalId)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this.Units[index][1] = (int) this.Units[index][1] + 1;
            }
            else
            {
                this.Units.Add(new JArray
                {
                    data.GlobalId,
                    1
                });

                this.SetLevel(data.GlobalId, this.Battle.Attacker.Player.HeroUpgrades.GetCountByData(data));
            }
        }

        internal void AlliancePortalDeployed()
        {
            for (int i = 0; i < this.Battle.Attacker.Player.AllianceUnits.Count; i++)
            {
                this.CastleUnits.Add(this.UnitSlotArrayToJSONArray(this.Battle.Attacker.Player.AllianceUnits[i]));
            }
        }
    }
}