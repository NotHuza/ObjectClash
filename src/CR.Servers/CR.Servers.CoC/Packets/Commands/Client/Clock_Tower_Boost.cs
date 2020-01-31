namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Clock_Tower_Boost : Command
    {
        internal int Id;

        public Clock_Tower_Boost(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 595;
            }
        }

        internal override void Decode()
        {
            this.Id = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}