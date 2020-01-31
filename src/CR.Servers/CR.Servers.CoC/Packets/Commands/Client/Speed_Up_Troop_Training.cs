namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Speed_Up_Troop_Training : Command
    {
        internal int Manager;

        public Speed_Up_Troop_Training(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 513;
            }
        }

        internal override void Decode()
        {
            this.Manager = this.Reader.ReadInt32();
            this.Reader.ReadByte();
            base.Decode();
        }
    }
}