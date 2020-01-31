namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using System.Linq;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Enums;

    internal class AchievementProgressSlot : DataSlots
    {
        internal Player Player;

        public AchievementProgressSlot(Player Player)
        {
            this.Player = Player;
        }

        internal void Refresh()
        {
            Data[] Datas = CSV.Tables.Get(Gamefile.Achievements).Datas.ToArray();
            for (int i = 0; i < Datas.Length; i++)
            {
                if (Datas[i] != null)
                {
                    if (Datas[i] is AchievementData)
                    {
                        AchievementData Achievement = (AchievementData)Datas[i];
                        switch (Achievement.Action)
                        {
                            case "upgrade":
                                {
                                    Data Data = CSV.Tables.Get(Gamefile.Buildings).GetData(Achievement.ActionData);
                                    if (Data != null)
                                    {
                                        if (Data is BuildingData)
                                        {
                                            BuildingData BuildingData = (BuildingData)Data;
                                            List<GameObject> Buildings =
                                                this.Player.Level.GameObjectManager.Filter.GetGameObjectsByData(
                                                    BuildingData);

                                            if (Buildings != null && Buildings.Count > 0)
                                            {
                                                int Level = (from Building Building in Buildings
                                                             select Building.GetUpgradeLevel()).Concat(new[] { 0 }).Max() +
                                                            1;

                                                this.Set(Achievement.GlobalId, Level);
                                                this.Set(Achievement.GlobalId + 1, Level);
                                                this.Set(Achievement.GlobalId + 2, Level);
                                            }
                                        }
                                    }

                                    i += 2;
                                    break;
                                }

                            case "unit_unlock":
                                {
                                    Data Data = CSV.Tables.Get(Gamefile.Characters).GetData(Achievement.ActionData);
                                    if (Data != null)
                                    {
                                        if (Data is CharacterData)
                                        {
                                            CharacterData CharacterData = (CharacterData)Data;
                                            bool Building;
                                            if (CharacterData.VillageType == 0)
                                            {
                                                if (CharacterData.UnitOfType == 1)
                                                {
                                                    Building = this.Player.Level.ComponentManager.MaxBarrackLevel + 1 >=
                                                               CharacterData.BarrackLevel;
                                                }
                                                else
                                                {
                                                    Building = this.Player.Level.ComponentManager.MaxDarkBarrackLevel + 1 >=
                                                               CharacterData.BarrackLevel;
                                                }
                                            }
                                            else
                                            {
                                                Building = this.Player.Level.Player.Variables.Village2BarrackLevel + 1 >=
                                                           CharacterData.BarrackLevel;
                                            }

                                            this.Set(Achievement.GlobalId, Building ? 1 : 0);
                                        }
                                    }

                                    break;
                                }

                            case "clear_obstacles":
                                {
                                    this.Set(Achievement.GlobalId, this.Player.ObstacleCleaned);
                                    this.Set(Achievement.GlobalId + 1, this.Player.ObstacleCleaned);
                                    this.Set(Achievement.GlobalId + 2, this.Player.ObstacleCleaned);

                                    i += 2;

                                    break;
                                }


                            case "league":
                                {
                                    this.Set(Achievement.GlobalId, this.Player.League);
                                    this.Set(Achievement.GlobalId + 1, this.Player.League);
                                    this.Set(Achievement.GlobalId + 2, this.Player.League);

                                    i += 2;
                                    break;
                                }

                            case "npc_stars":
                                {
                                    int Stars = this.Player.NpcMapProgress.Sum(T => T.Count);

                                    this.Set(Achievement.GlobalId, Stars);
                                    this.Set(Achievement.GlobalId + 1, Stars);
                                    this.Set(Achievement.GlobalId + 2, Stars);

                                    i += 2;
                                    break;
                                }

                            case "donate_units":
                                {
                                    this.Set(Achievement.GlobalId, this.Player.Donation);
                                    this.Set(Achievement.GlobalId + 1, this.Player.Donation);
                                    this.Set(Achievement.GlobalId + 2, this.Player.Donation);

                                    i += 2;
                                    break;
                                }

                            case "gear_up":
                                {
                                    GameObject[] Buildings = this.Player.Level.GameObjectManager.GameObjects[0][0].ToArray();
                                    int GearUp = Buildings.Cast<Building>().Count(Building => Building?.CombatComponent?.GearUp > 0);

                                    this.Set(Achievement.GlobalId, GearUp);
                                    this.Set(Achievement.GlobalId + 1, GearUp);
                                    this.Set(Achievement.GlobalId + 2, GearUp);

                                    i += 2;
                                    break;
                                }

                            case "repair_building":
                                {
                                    Data Data = CSV.Tables.Get(Gamefile.Buildings).GetData(Achievement.ActionData);
                                    if (Data != null)
                                    {
                                        if (Data is BuildingData)
                                        {
                                            BuildingData BuildingData = (BuildingData)Data;
                                            List<GameObject> Buildings = this.Player.Level.GameObjectManager.Filter.GetGameObjectsByData(BuildingData);
                                            if (Buildings != null && Buildings.Count > 0)
                                            {
                                                GameObject Gameobject = Buildings[0];
                                                if (Gameobject is Building)
                                                {
                                                    Building Building = (Building)Gameobject;
                                                    if (!Building.Locked)
                                                    {
                                                        this.Set(Achievement.GlobalId, 1);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
        }

        internal bool Claim(int GlobalId)
        {
            this.Refresh();

            Data Data = CSV.Tables.Get(Gamefile.Achievements).GetDataWithID(GlobalId);
            if (Data != null)
            {
                if (Data is AchievementData)
                {
                    AchievementData Achievement = (AchievementData)Data;
                    if (this.GetCountByData(Achievement) >= Achievement.ActionCount)
                    {
                        if (this.Player.Achievements.Add(GlobalId))
                        {
                            this.Player.AddDiamonds(Achievement.DiamondReward);
                            this.Player.AddExperience(Achievement.ExpReward);

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}