namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Speed_Up_Troop_Training_V2 : Command
    {
        internal int BuildingId;

        public Speed_Up_Troop_Training_V2(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 596;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}