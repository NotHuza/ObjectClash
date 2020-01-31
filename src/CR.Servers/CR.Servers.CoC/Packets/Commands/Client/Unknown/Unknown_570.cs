namespace CR.Servers.CoC.Packets.Commands.Client.Unknown
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Unknown_570 : Command
    {
        public Unknown_570(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 570;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            base.Decode();
        }
    }
}