namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class New_Shop_Seen : Command
    {
        public New_Shop_Seen(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 532;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            base.Decode();
        }
    }
}