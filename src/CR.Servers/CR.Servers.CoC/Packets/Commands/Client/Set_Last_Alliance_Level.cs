namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Set_Last_Alliance_Level : Command
    {
        internal int AllianceLevel;

        public Set_Last_Alliance_Level(Device device, Reader reader) : base(device, reader)
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
            this.AllianceLevel = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}