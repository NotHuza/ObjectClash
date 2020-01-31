namespace CR.Servers.CoC.Packets.Commands.Client.Unknown
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Unknown_581 : Command
    {
        internal int Count;

        public Unknown_581(Device Device, Reader Reader) : base(Device, Reader)
        {
            //Seems to be war related
        }

        internal override int Type
        {
            get
            {
                return 581;
            }
        }

        internal override void Decode()
        {
            this.Count = this.Reader.ReadInt32();

            for (int i = 0; i < this.Count; i++)
            {
                this.Reader.ReadInt64();
            }

            this.Reader.ReadInt64();
            this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            base.Decode();
        }
    }
}