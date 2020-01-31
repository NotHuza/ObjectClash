namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class AvailableServerCommandMessage : Message
    {
        internal Command Command;

        public AvailableServerCommandMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24111;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(this.Command.Type);
            this.Command.Encode(this.Data);
        }
    }
}