namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Commands.Client.List;
    using CR.Servers.Extensions.Binary;

    internal class Move_Multiple_Buildings : Command
    {
        internal List<BuildingToMove> Buildings;

        public Move_Multiple_Buildings(Device Client, Reader Reader) : base(Client, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 533;
            }
        }

        internal override void Decode()
        {
            int buildingCount = this.Reader.ReadInt32();
            this.Buildings = new List<BuildingToMove>(buildingCount);

            for (int i = 0; i < buildingCount; i++)
            {
                this.Buildings.Add(new BuildingToMove
                {
                    X = this.Reader.ReadInt32(),
                    Y = this.Reader.ReadInt32(),
                    Id = this.Reader.ReadInt32()
                });
            }

            base.Decode();
        }

        internal override void Execute()
        {
            foreach (BuildingToMove building in this.Buildings)
            {
                GameObject GameObject = this.Device.GameMode.Level.GameObjectManager.Filter.GetGameObjectByPreciseId(building.Id);

                if (GameObject != null)
                {
                    if (GameObject.Type != 3)
                    {
                        GameObject.SetPositionXY(building.X, building.Y);
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to move the building. The game object is an obstacle");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to move the building. The game object is null");
                }
            }
        }
    }
}