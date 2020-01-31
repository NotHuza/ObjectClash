namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Set_Active_Layout : Command
    {
        internal int Layout;

        public Set_Active_Layout(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 567;
            }
        }

        internal override void Decode()
        {
            this.Layout = this.Reader.ReadInt32();
            this.Reader.ReadInt32();
            base.Decode();
        }
    }
}