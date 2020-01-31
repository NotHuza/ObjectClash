namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Account_Bound : Command
    {
        public Account_Bound(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 603;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            //TODO: Execute this command
        }
    }
}