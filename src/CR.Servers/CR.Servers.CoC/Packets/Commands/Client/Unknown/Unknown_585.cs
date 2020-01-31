namespace CR.Servers.CoC.Packets.Commands.Client.Unknown
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Unknown_585 : Command
    {
        public Unknown_585(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 585;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadByte();
            this.Reader.ReadInt32();
            base.Decode();
        }
    }
}