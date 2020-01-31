namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Send_Alliance_Challenge : Command
    {
        public Send_Alliance_Challenge(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 574;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadString();
            this.Reader.ReadByte();
            this.Reader.ReadByte();
            base.Decode();
        }
    }
}