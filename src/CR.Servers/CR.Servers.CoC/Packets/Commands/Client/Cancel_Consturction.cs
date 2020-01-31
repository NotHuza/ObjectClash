namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Cancel_Consturction : Command
    {
        internal int BuildingID;

        public Cancel_Consturction(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 505;
            }
        }

        internal override void Decode()
        {
            this.BuildingID = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            GameObject GameObject = Level.GameObjectManager.Filter.GetGameObjectByPreciseId(this.BuildingID);

            if (GameObject != null)
            {
                if (GameObject is Building )
                {
                    Building Building = (Building)GameObject;
                    if (!Building.Constructing)
                    {
                        Logging.Error(this.GetType(), $"Tried to cancel the construction of a building which is not in construction with game ID {this.BuildingID}.");
                    }
                    else
                    {
                        Building.CancelConstruction();
                    }
                }
                else if (GameObject is Trap)
                {
                    Trap Trap = (Trap)GameObject;
                    if (!Trap.Constructing)
                    {
                        Logging.Error(this.GetType(), $"Tried to cancel the construction of a trap which is not in construction with game ID {this.BuildingID}.");
                    }
                    else
                    {
                        Trap.CancelConstruction();
                    }
                }
                else if (GameObject is Obstacle )
                {
                    Obstacle Obstacle = (Obstacle)GameObject;
                    if (!Obstacle.ClearingOnGoing)
                    {
                        Logging.Error(this.GetType(), $"Tried to cancel the clearing of an obstacle which is not in clearing with game ID {this.BuildingID}.");
                    }
                    else
                    {
                        Obstacle.CancelClearing();
                    }
                }
            }
            else
            {
                Logging.Error(this.GetType(), $"Unable to cancel the construction. The game object is null with game Id {this.BuildingID}.");
            }
        }
    }
}