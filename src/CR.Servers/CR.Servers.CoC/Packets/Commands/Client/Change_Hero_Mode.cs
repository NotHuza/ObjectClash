namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Change_Hero_Mode : Command
    {
        internal int BuildingId;
        internal int State;

        public Change_Hero_Mode(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 572;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            this.State = this.Reader.ReadInt32();
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
                        HeroData HeroData = HeroBaseComponent.HeroData;
                        if (HeroData != null)
                        {
                            if (HeroData.HasAltMode)
                            {
                                level.Player.HeroModes.Set(HeroData, this.State);
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Unable to change hero mode. Hero doesn't have another mode .");
                            }
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to change hero mode. Hero data is null.");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to change hero mode. The HeroBaseComponent is null.");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to change hero mode. The game object is not a building.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to change hero mode. The game object is null.");
            }
        }
    }
}