namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Train_Quick_Units : Command
    {
        internal int Slot;

        public Train_Quick_Units(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 559;
            }
        }

        internal override void Decode()
        {
            this.Slot = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}