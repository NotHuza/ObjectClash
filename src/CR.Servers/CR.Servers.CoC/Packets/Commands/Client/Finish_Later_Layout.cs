namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Finish_Later_Layout : Command
    {
        internal int Layout;

        public Finish_Later_Layout(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 548;
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