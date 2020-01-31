namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Upgrade_Unit : Command
    {
        internal int BuildingId;

        internal Data Unit;
        internal int UnitType;

        public Upgrade_Unit(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 516;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            this.UnitType = this.Reader.ReadInt32();
            this.Unit = this.Reader.ReadData();
            base.Decode();
        }

        internal override void Execute()
        {
            Level level = this.Device.GameMode.Level;
            if (this.Unit != null)
            {
                GameObject gameObject = level.GameObjectManager.Filter.GetGameObjectById(this.BuildingId);
                if (gameObject != null)
                {
                    if (gameObject is Building)
                    {
                        Building building = (Building)gameObject;
                        UnitUpgradeComponent unitUpgradeComponent = building.UnitUpgradeComponent;

                        if (unitUpgradeComponent != null)
                        {
                            int unitLevel = level.Player.GetUnitUpgradeLevel(this.Unit);

                            ResourceData resourceData = this.UnitType == 1 ? ((SpellData) this.Unit).UpgradeResourceData : ((CharacterData) this.Unit).UpgradeResourceData;
                            int upgradeCost = this.UnitType == 1 ? ((SpellData) this.Unit).UpgradeCost[unitLevel] : ((CharacterData) this.Unit).UpgradeCost[unitLevel];

                            if (resourceData != null)
                            {
                                if (level.Player.Resources.GetCountByData(resourceData) >= upgradeCost)
                                {
                                    if (unitUpgradeComponent.CanStartUpgrading(this.Unit))
                                    {
                                        level.Player.Resources.Remove(resourceData, upgradeCost);
                                        unitUpgradeComponent.StartUpgrading(this.Unit);
                                    }
                                    else
                                    {
                                        Logging.Error(this.GetType(),
                                            "Unable to upgrade the unit. The UnitUpgradeComponent probably training other unit.");
                                    }
                                }
                                else
                                {
                                    Logging.Error(this.GetType(),
                                        "Unable to upgrade the unit. The player doesn't have enough resources.");
                                }
                            }
                            else
                            {
                                Logging.Error(this.GetType(),
                                    "Unable to upgrade the unit. The resources data is null.");
                            }
                        }
                        else
                        {
                            Logging.Error(this.GetType(),
                                "Unable to upgrade the unit. The game object doesn't contain a UnitUpgradeComponent.");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(),
                            "Unable to upgrade the unit. The game object is not valid or not exist");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to upgrade the unit. The game object is null");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to to upgrade the unit. The unit data is null");
            }
        }
    }
}