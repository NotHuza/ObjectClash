namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Change_Hero_State : Command
    {
        internal int BuildingId;
        internal bool Sleeping;

        public Change_Hero_State(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 529;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            this.Sleeping = this.Reader.ReadBoolean();
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
                            level.Player.HeroStates.Set(HeroData, this.Sleeping ? 2 : 3);
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to change hero state. Hero data is null.");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to change hero state. The HeroBaseComponent is null.");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to change hero state. The game object is not a building.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to change hero state. The game object is null.");
            }
        }
    }
}