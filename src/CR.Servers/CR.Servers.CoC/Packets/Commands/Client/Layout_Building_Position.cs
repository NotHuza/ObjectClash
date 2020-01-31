namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Commands.Client.List;
    using CR.Servers.Extensions.Binary;

    internal class Layout_Building_Position : Command
    {
        internal List<BuildingToMove> Buildings;

        public Layout_Building_Position(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 598;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
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
    }
}