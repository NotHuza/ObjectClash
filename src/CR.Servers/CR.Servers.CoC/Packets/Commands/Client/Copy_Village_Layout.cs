namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Copy_Village_Layout : Command
    {
        internal int From;
        internal int To;

        public Copy_Village_Layout(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 568;
            }
        }

        internal override void Decode()
        {
            base.Decode();
            this.From = this.Reader.ReadInt32();
            this.To = this.Reader.ReadInt32();
        }
    }
}