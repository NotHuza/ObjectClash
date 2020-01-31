namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class News_Seen : Command
    {
        internal int NewsId;

        public News_Seen(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 539;
            }
        }

        internal override void Decode()
        {
            this.NewsId = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}