namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Commands.Client.List;
    using CR.Servers.Extensions.Binary;

    internal class Buy_Walls : Command
    {
        internal BuildingData BuildingData;

        internal int Count;
        internal List<BuildingToMove> WallXy;

        public Buy_Walls(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 590;
            }
        }

        internal override void Decode()
        {
            this.Count = this.Reader.ReadInt32();
            this.WallXy = new List<BuildingToMove>(this.Count);

            for (int i = 0; i < this.Count; i++)
            {
                this.WallXy.Add(new BuildingToMove
                {
                    X = this.Reader.ReadInt32(),
                    Y = this.Reader.ReadInt32()
                });
            }

            this.BuildingData = this.Reader.ReadData<BuildingData>();
            base.Decode();
        }

        internal override void Execute()
        {
            if (this.BuildingData != null)
            {
                Level Level = this.Device.GameMode.Level;
                //if (!Level.IsBuildingCapReached(this.BuildingData))
                {
                    BuildingClassData BuildingClassData = (BuildingClassData) CSV.Tables.Get(Gamefile.Building_Classes).GetData(this.BuildingData.BuildingClass);
                    ResourceData ResourceData = this.BuildingData.BuildResourceData;

                    if (BuildingClassData.CanBuy)
                    {
                        if (this.BuildingData.TownHallLevel2[0] <= Level.GameObjectManager.TownHall2.GetUpgradeLevel() + 1)
                        {
                            if (Level.Player.Resources.GetCountByData(ResourceData) >= this.BuildingData.BuildCost[0])
                            {
                                if (Level.WorkerManagerV2.FreeWorkers > 0)
                                {
                                    Level.Player.Resources.Remove(ResourceData, this.BuildingData.BuildCost[0]);
                                    Level.Player.WallGroupId++;

                                    foreach (BuildingToMove Xy in this.WallXy)
                                    {
                                        this.StartConstruction(Level, Xy);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void StartConstruction(Level Level, BuildingToMove Xy)
        {
            Building GameObject = new Building(this.BuildingData, Level);
            CombatComponent CombatComponent = GameObject.CombatComponent;

            GameObject.SetUpgradeLevel(-1);

            GameObject.Position.X = Xy.X << 9;
            GameObject.Position.Y = Xy.Y << 9;

            if (CombatComponent != null)
            {
                CombatComponent.WallI = Level.Player.WallGroupId;
            }
            else
            {
                Logging.Error(this.GetType(), "CombatComponent should not be null when buying multiple wall!");
            }

            Level.WorkerManagerV2.AllocateWorker(GameObject);

            if (this.BuildingData.GetBuildTime(0) <= 0)
            {
                GameObject.FinishConstruction();
            }
            else
            {
                GameObject.ConstructionTimer = new Timer();
                GameObject.ConstructionTimer.StartTimer(Level.Player.LastTick, this.BuildingData.GetBuildTime(0));
            }

            Level.GameObjectManager.AddGameObject(GameObject);
        }
    }
}