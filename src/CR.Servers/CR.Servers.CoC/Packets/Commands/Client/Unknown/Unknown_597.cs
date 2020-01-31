namespace CR.Servers.CoC.Packets.Commands.Client.Unknown
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Unknown_597 : Command
    {
        internal int Timestamp;

        public Unknown_597(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 597;
            }
        }

        internal override void Decode()
        {
            this.Timestamp = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}