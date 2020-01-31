namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Text;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Upgrade_Hero : Command
    {
        internal int BuildingId;

        public Upgrade_Hero(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 527;
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
                    HeroBaseComponent HeroBaseComponent = building.HeroBaseComponent;
                    if (HeroBaseComponent != null)
                    {
                        if (HeroBaseComponent.UpgradeAvailable)
                        {
                            HeroData HeroData = HeroBaseComponent.HeroData;
                            int heroLevel = level.Player.GetHeroUpgradeLevel(HeroData);

                            ResourceData resourceData = HeroData.UpgradeResourceData;
                            int upgradeCost = HeroData.UpgradeCost[heroLevel];

                            if (resourceData != null)
                            {
                                if (level.Player.Resources.GetCountByData(resourceData) >= upgradeCost)
                                {
                                    if (HeroData.VillageType == 0
                                        ? level.WorkerManager.FreeWorkers > 0
                                        : level.WorkerManagerV2.FreeWorkers > 0)
                                    {
                                        level.Player.Resources.Remove(resourceData, upgradeCost);
                                        HeroBaseComponent.StartUpgrade();
                                    }
                                    else
                                    {
                                        Logging.Error(this.GetType(),
                                            "Unable to upgrade the hero. There is no free worker.");
                                    }
                                }
                                else
                                {
                                    Logging.Error(this.GetType(),
                                        "Unable to upgrade the hero. The player doesn't have enough resources.");
                                }
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Unable to upgrade the hero. Resource data is null.");
                            }
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to upgrade the hero. Upgrade is not available.");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to upgrade the hero. The HeroBaseComponent is null.");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to upgrade the hero. The game object is not a building.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to upgrade the hero. The game object is null.");

                StringBuilder Error = new StringBuilder();
                GameObject Precise = this.Device.GameMode.Level.GameObjectManager.Filter.GetGameObjectByPreciseId(this.BuildingId);
                if (Precise != null)
                {
                    Error.AppendLine($"Building Id :  {this.BuildingId}");
                    Error.AppendLine($"Building Name :  {Precise.Data.Name}");
                    Error.AppendLine($"Building still null with precise id : {false}");

                    Error.AppendLine($"Player Id :  {this.Device.GameMode.Level.Player.UserId}");
                    Error.AppendLine($"Player current village :  {this.Device.GameMode.Level.GameObjectManager.Map}");
                    Error.AppendLine($"Player town hall level :  {this.Device.GameMode.Level.Player.TownHallLevel}");
                    Error.AppendLine($"Player town hall2 level :  {this.Device.GameMode.Level.Player.TownHallLevel2}");
                }
                else
                {
                    Error.AppendLine($"Precise building : null");
                    Error.AppendLine($"Building Id :  {this.BuildingId}");

                    Error.AppendLine($"Player Id :  {this.Device.GameMode.Level.Player.UserId}");
                    Error.AppendLine($"Player current village :  {this.Device.GameMode.Level.GameObjectManager.Map}");
                    Error.AppendLine($"Player town hall level :  {this.Device.GameMode.Level.Player.TownHallLevel}");
                    Error.AppendLine($"Player town hall2 level :  {this.Device.GameMode.Level.Player.TownHallLevel2}");
                }

                Resources.Logger.Debug(Error);
            }
        }
    }
}