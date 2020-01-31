namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Open_Close_Layout : Command
    {
        internal bool FinishLater;

        internal int Layout;
        internal int State;

        public Open_Close_Layout(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 552;
            }
        }

        internal override void Decode()
        {
            this.Layout = this.Reader.ReadInt32();
            this.State = this.Reader.ReadInt32();
            this.FinishLater = this.Reader.ReadBoolean();
            base.Decode();
        }
    }
}