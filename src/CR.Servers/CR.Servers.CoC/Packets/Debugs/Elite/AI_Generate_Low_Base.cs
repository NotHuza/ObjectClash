namespace CR.Servers.CoC.Packets.Debugs.Elite
{
    using System;
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Logic.Map;
    using CR.Servers.CoC.Packets.Messages.Server.Avatar;
    using CR.Servers.Logic.Enums;
    using Newtonsoft.Json.Linq;

    internal class AI_Generate_Low_Base : Debug
    {
        internal List<Building> Buildings;
        internal TileMap TileMap;

        public AI_Generate_Low_Base(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            this.Buildings = new List<Building>();
            this.TileMap = new TileMap(50, 50);
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Elite;
            }
        }

        internal override void Process()
        {
            bool Valid = true;
            bool AltMode = false;
            if (this.Parameters.Length >= 1)
            {
                int Id;
                if (int.TryParse(this.Parameters[0], out Id))
                {
                    if (this.Parameters.Length >= 2)
                    {
                        Valid = bool.TryParse(this.Parameters[1], out AltMode);
                    }

                    if (Valid)
                    {
                        this.Device.GameMode.Level.Player.ModSlot.AILevel = new Level(true);
                        Level AI = this.Device.GameMode.Level.Player.ModSlot.AILevel;

                        var data = CSV.Tables.Get(Gamefile.Buildings).GetDataWithInstanceID(Id);
                        if (data is BuildingData)
                        {
                            BuildingData BuildingData = (BuildingData)data;
                            if (BuildingData.IsWall)
                            {
                                var TownHallData = CSV.Tables.Get(Gamefile.Buildings).GetDataWithID(1000001) as BuildingData;

                                Building TownHall = new Building(TownHallData, AI)
                                {
                                    Position =
                                    {
                                        X = (24) << 9,
                                        Y = (24) << 9,
                                    },
                                    Id = GlobalId.Create(500, this.Buildings.Count),
                                };

                                TownHall.SetUpgradeLevel(TownHallData.MaxLevel);

                                this.Buildings.Add(TownHall);
                                this.TileMap.AddGameObject(TownHall);
                            }

                            if (this.Parameters.Length < 2 || !AltMode || BuildingData.AltAttackMode)
                            {
                                for (int X = 3; X <= 46;)
                                {
                                    for (int Y = 3; Y <= 46;)
                                    {
                                        if (this.IsValidPlaceForBuilding(BuildingData, X, Y, BuildingData.Width, BuildingData.Height))
                                        {
                                            Building Building = new Building(BuildingData, AI)
                                            {
                                                Position =
                                                {
                                                    X = X << 9,
                                                    Y = Y << 9
                                                },
                                                Id = GlobalId.Create(500, this.Buildings.Count)
                                            };

                                            Building.SetUpgradeLevel(1);

                                            if (Building.CombatComponent != null)
                                            {
                                                if (AltMode)
                                                {
                                                    if (BuildingData.AltAttackMode)
                                                    {
                                                        if (!string.IsNullOrEmpty(BuildingData.GearUpBuilding))
                                                        {
                                                            Building.CombatComponent.GearUp = 1;
                                                        }

                                                        Building.CombatComponent.AttackMode = true;
                                                        Building.CombatComponent.AttackModeDraft = true;
                                                    }
                                                }
                                            }

                                            if (Building.HeroBaseComponent != null)
                                            {
                                                HeroData HeroData = Building.HeroBaseComponent.HeroData;
                                                AI.Player.HeroUpgrades.Set(HeroData.GlobalId, 0);
                                            }

                                            this.Buildings.Add(Building);
                                            this.TileMap.AddGameObject(Building, 0);

                                            //Logging.Info(this.GetType(), "X:" + X + "   Y:" + Y);
                                            break;
                                        }

                                        if (Y + BuildingData.Width < 47)
                                        {
                                            Y += BuildingData.Width;
                                        }
                                        else
                                        {
                                            Y = 47;
                                            if (X + BuildingData.Height < 47)
                                            {
                                                X += BuildingData.Height;
                                            }
                                            else
                                            {
                                                X = 47;
                                                break;
                                            }
                                        }
                                    }
                                }

                                this.SendChatMessage("AI Base generated, Enjoy!");
                                this.Device.GameMode.Level.Player.ModSlot.AIAttack = true;
                                AI.Home.LastSave = this.Save();
                            }
                            else
                            {
                                this.SendChatMessage("Unable to generate AI Base. The building doesn't have alt mode!.");
                            }
                        }
                        else
                        {
                            this.SendChatMessage("Unable to generate AI Base. You may have entered unknown id causing building data to be null.");
                            this.SendChatMessage("If you have confirmed the id is valid and the issue persists, please contact our development team ASAP");
                        }
                    }
                    else
                    {
                        this.SendChatMessage("Unable to generate AI Base. You have entered unknown value after building id.");
                    }
                }
                else
                {
                    this.SendChatMessage("Unable to generate AI Base. You have entered invalid id!.");
                }
            }
            else
            {
                new AvatarStreamEntryMessage(this.Device)
                {
                    StreamEntry = new AllianceMailAvatarStreamEntry(this.Device.GameMode.Level.Player)
                    {
                        LowId = (int) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        SenderName = "[System] Command Manager",
                        SenderLeague = 22,
                        Message = Constants.AIBaseHelp.ToString()
                    }
                }.Send();

                this.SendChatMessage("Please check your MailBox!");
            }
        }

        internal JObject Save()
        {
            JObject Json = new JObject();
            JArray Buildings = new JArray();

            foreach (GameObject GameObject in this.Buildings)
            {
                JObject Token = new JObject();

                if (GameObject.Data != null)
                {
                    Token.Add("data", GameObject.Data.GlobalId);
                    Token.Add("id", GameObject.Id);

                    GameObject.Save(Token);
                    Buildings.Add(Token);
                }
            }

            Json.Add("exp_ver", 1);
            Json.Add("buildings", Buildings);
            Json.Add("obstacles", new JArray());
            Json.Add("traps", new JArray());
            Json.Add("decos", new JArray());
            Json.Add("vobjs", new JArray());
            Json.Add("buildings2", new JArray());
            Json.Add("obstacles2", new JArray());
            Json.Add("traps2", new JArray());
            Json.Add("decos2", new JArray());
            Json.Add("vobjs2", new JArray());

            return Json;
        }

        internal bool IsValidPlaceForBuilding(BuildingData Data, int X, int Y, int Width, int Height)
        {
            bool Valid = false;

            if (X >= 0 && Y >= 0)
            {
                if (Width + X <= 50 && Height + Y <= 50)
                {
                    Valid = true;

                    if (Width > 0 && Height > 0)
                    {
                        for (int i = 0; i < Width; i++)
                        {
                            for (int j = 0; j < Height; j++)
                            {
                                if (TileMap.Exists(X + i, Y + j, 0))
                                {
                                    Tile Tile = this.TileMap[X + i, Y + j, 0];
                                    if (!Tile.IsBuildable())
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return Valid;
        }

        public override void Dispose()
        {
            base.Dispose();

            if (this.Buildings != null)
            {
                if (this.Buildings.Count != 0)
                {
                    do
                    {
                        this.Buildings.RemoveAt(0);
                    } while (this.Buildings.Count != 0);
                }

                this.Buildings = null;
            }

            if (this.TileMap != null)
            {
                this.TileMap.Destruct();
                this.TileMap = null;
            }
        }
    }
}