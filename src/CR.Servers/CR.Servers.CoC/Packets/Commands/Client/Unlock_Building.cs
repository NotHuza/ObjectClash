namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Unlock_Building : Command
    {
        internal int BuildingId;

        public Unlock_Building(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 520;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            Level level = this.Device.GameMode.Level;

            GameObject gameObject = level.GameObjectManager.Filter.GetGameObjectById(this.BuildingId);
            if (gameObject != null)
            {
                if (gameObject is Building)
                {
                    Building building = (Building)gameObject;
                    if (building.Locked)
                    {
                        BuildingData data = building.BuildingData;
                        ResourceData resourceData = data.BuildResourceData;
                        if (level.Player.Resources.GetCountByData(resourceData) >= data.BuildCost[0])
                        {
                            if (data.VillageType == 0)
                            {
                                level.Player.Resources.Remove(resourceData, data.BuildCost[0]);

                                if (data.IsAllianceCastle)
                                {
                                    level.Player.SetAllianceCastleLevel(0);
                                }

                                building.Locked = false;
                            }
                            else
                            {
                                if (level.WorkerManagerV2.FreeWorkers > 0)
                                {
                                    level.Player.Resources.Remove(resourceData, data.BuildCost[0]);
                                    building.SetUpgradeLevel(-1);

                                    level.WorkerManagerV2.AllocateWorker(building);

                                    if (data.GetBuildTime(0) <= 0)
                                    {
                                        building.FinishConstruction();
                                    }
                                    else
                                    {
                                        building.ConstructionTimer = new Timer();
                                        building.ConstructionTimer.StartTimer(level.Player.LastTick, data.GetBuildTime(0));
                                    }
                                }
                                else
                                {
                                    Logging.Error(this.GetType(),
                                        "Unable to unlock the building. The player doesn't have any free worker.");
                                }
                            }
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to unlock the building. The player doesn't have enough resources.");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to unlock the building. The building is already unlocked");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to unlock the building. The game object is not a building.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to unlock the building. The game object is null");
            }
        }
    }
}