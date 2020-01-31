namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Boost_Buildings_Of_Type : Command
    {
        internal int BuildingType;

        public Boost_Buildings_Of_Type(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 584;
            }
        }

        internal override void Decode()
        {
            this.BuildingType = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}