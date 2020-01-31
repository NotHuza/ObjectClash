namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Speed_Up_Hero_Upgrade : Command
    {
        internal int BuildingId;
        internal int Village;

        public Speed_Up_Hero_Upgrade(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 528;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            this.Village = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            GameObject gameObject = this.Device.GameMode.Level.GameObjectManager.Filter.GetGameObjectById(this.BuildingId);

            if (gameObject != null)
            {
                if (gameObject is Building)
                {
                    Building building = (Building)gameObject;
                    HeroBaseComponent HeroBaseComponent = building.HeroBaseComponent;

                    if (HeroBaseComponent != null)
                    {
                        HeroBaseComponent.SpeedUpUpgrade();
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to speed up the hero upgrade. The game object doesn't contain a UnitUpgradeComponent.");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to speed up the hero upgrade. The game object is not a building.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to speed up the hero upgrade. The game object is null");
            }
        }
    }
}