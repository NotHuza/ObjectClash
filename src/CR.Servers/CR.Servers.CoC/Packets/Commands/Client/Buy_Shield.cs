namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Buy_Shield : Command
    {
        internal int ShieldId;

        public Buy_Shield(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 522;
            }
        }

        internal override void Decode()
        {
            this.ShieldId = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}