namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Move_Building_In_Layout : Command
    {
        internal int BuildingId;
        internal int Layout;

        internal int X;
        internal int Y;

        public Move_Building_In_Layout(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 546;
            }
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.BuildingId = this.Reader.ReadInt32();
            this.Layout = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}