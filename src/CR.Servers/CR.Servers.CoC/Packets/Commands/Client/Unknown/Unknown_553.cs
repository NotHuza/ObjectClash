namespace CR.Servers.CoC.Packets.Commands.Client.Unknown
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Unknown_553 : Command
    {
        internal int Unknown1;

        public Unknown_553(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 553;
            }
        }

        internal override void Decode()
        {
            this.Unknown1 = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}