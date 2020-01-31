namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Speed_Up_Construction : Command
    {
        internal int BuildingId;
        internal int Village;

        public Speed_Up_Construction(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 504;
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
            GameObject GameObject = this.Device.GameMode.Level.GameObjectManager.Filter.GetGameObjectById(this.BuildingId);

            if (GameObject != null)
            {
                if (GameObject is Building)
                {
                    Building Building = (Building)GameObject;
                    Building.SpeedUpConstruction();
                }
                else if (GameObject is Trap)
                {
                    Trap Trap = (Trap)GameObject;
                    Trap.SpeedUpConstruction();
                }
                else if (GameObject is VillageObject)
                {
                    VillageObject VillageObject = (VillageObject)GameObject;
                    VillageObject.SpeedUpConstruction();
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to speed up the construction. GameObject Type : " + GameObject.Type + ".");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to speed up the construction. The game object doesn't exist.");
            }
        }
    }
}