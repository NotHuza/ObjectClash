namespace CR.Servers.CoC.Packets.Debugs
{
    using System.IO;
    using System.Text;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Logic.Enums;
    using Newtonsoft.Json.Linq;

    internal class Set_Bases : Debug
    {
        internal int Level;
        public Set_Bases(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Set_Bases
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Player;              
            }
        }

        internal override void Process()
        {
            Player Player = this.Device.GameMode.Level.Player;
            GameObjectManager GameObjectManager = this.Device.GameMode.Level.GameObjectManager;

            if (this.Parameters.Length >= 1)
            {
                if (int.TryParse(this.Parameters[0], out this.Level))
                {
                    if (Player.TownHallLevel2 > 0)
                    {
                        JObject Village;
                        if (!LevelFile.TownHallTemplate.TryGetValue(this.Level, out Village))
                        {
                            if (File.Exists($"Gamefiles/level/townhall{this.Level}.json"))
                            {
                                Village = JObject.Parse(File.ReadAllText($"Gamefiles/level/townhall{this.Level}.json", Encoding.UTF8));
                                LevelFile.TownHallTemplate.Add(this.Level, Village);
                            }
                            else
                            {
                                this.SendChatMessage("TH11 is the max!");
                                return;
                            }
                        }

                        var WorkerManager = this.Device.GameMode.Level.WorkerManager;
                        if (WorkerManager.GameObjects.Count != 0)
                        {
                            do
                            {
                                WorkerManager.GameObjects.Remove(0);
                            } while (WorkerManager.GameObjects.Count != 0);

                        }

                        var WorkerManagerV2 = this.Device.GameMode.Level.WorkerManagerV2;
                        if (WorkerManagerV2.GameObjects.Count != 0)
                        {
                            do
                            {
                                WorkerManagerV2.GameObjects.Remove(0);
                            } while (WorkerManagerV2.GameObjects.Count != 0);

                        }

                        for (int i = 0; i < GameObjectManager.GameObjects.Length; i++)
                        {
                            GameObjectManager.GameObjects[i][0].Clear();
                            GameObjectManager.GameObjects[i][1].Clear();
                        }

                        GameObjectManager.ObstaclesIndex[0] = 0;
                        GameObjectManager.ObstaclesIndex[1] = 0;

                        GameObjectManager.DecoIndex[0] = 0;
                        GameObjectManager.DecoIndex[1] = 0;

                        Player.Units2.Clear();

                        Player.Level.Load(Village);

                        if (this.Level >= 4)
                        {
                            GameObjectManager.Boat.SetUpgradeLevel(GameObjectManager.Boat.VillageObjectData.MaxLevel);
                        }

                        foreach (GameObject GameObject in GameObjectManager.Filter.GetGameObjects(0, 0))
                        {
                            Building Building = (Building) GameObject;
                            if (Building.HeroBaseComponent != null)
                            {
                                var data = CSV.Tables.Get(Gamefile.Heroes).GetData(Building.BuildingData.HeroType);
                                if (data is HeroData)
                                {
                                    HeroData HeroData = (HeroData)data;
                                    if (Player.HeroUpgrades.GetByGlobalId(HeroData.GlobalId) == null)
                                    {
                                        Player.HeroUpgrades.Set(HeroData, 0);
                                        Player.HeroStates.Set(HeroData, 3);

                                        if (HeroData.HasAltMode)
                                        {
                                            Player.HeroModes.Set(HeroData, 0);
                                        }
                                    }
                                }
                            }
                        }

                        foreach (GameObject GameObject in GameObjectManager.Filter.GetGameObjects(0, 1))
                        {
                            Building Building = (Building) GameObject;

                            if (Building.UnitStorageV2Component != null)
                            {
                                if (Building.UnitStorageV2Component.Units?.Count > 0)
                                {
                                    var data = CSV.Tables.GetWithGlobalId(Building.UnitStorageV2Component.Units[0].Data);
                                    if (data is CharacterData)
                                    {
                                        CharacterData UnitData = (CharacterData)data;

                                        int UnitLevel = Player.GetUnitUpgradeLevel(UnitData);
                                        int UnitCount = UnitData.UnitsInCamp[UnitLevel];
                                        Player.Units2.Add(UnitData, UnitCount);
                                        Building.UnitStorageV2Component.Units[0].Count = UnitCount;
                                    }
                                }
                            }
                            else if (Building.HeroBaseComponent != null)
                            {
                                var data = CSV.Tables.Get(Gamefile.Heroes).GetData(Building.BuildingData.HeroType);
                                if (data is HeroData)
                                {
                                    HeroData HeroData = (HeroData)data;
                                    if (Player.HeroUpgrades.GetByGlobalId(HeroData.GlobalId) == null)
                                    {
                                        Player.HeroUpgrades.Set(HeroData, 0);
                                        Player.HeroStates.Set(HeroData, 3);
                                        if (HeroData.HasAltMode)
                                        {
                                            Player.HeroModes.Set(HeroData, 0);
                                        }
                                    }
                                }
                            }
                        }

                        new OwnHomeDataMessage(this.Device).Send();
                    }
                    else
                    {
                        this.SendChatMessage("Please visit the builder village first before running this command!");
                    }
                }
                else
                {
                    this.SendChatMessage("Invalid parameters! Usage: /setbases <TH_LEVEL>");
                }
            }
            else
            {
                this.SendChatMessage("Missing parameters! Usage: /setbases <TH_LEVEL>");
            }
        }
    }
}