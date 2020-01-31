namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Cancel_Troop_Training : Command
    {
        internal int UnitCount;

        internal int UnitId;

        public Cancel_Troop_Training(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 509;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            this.UnitId = this.Reader.ReadInt32();
            this.UnitCount = this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            base.Decode();
        }
    }
}