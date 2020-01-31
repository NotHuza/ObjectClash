namespace CR.Servers.CoC.Packets.Messages.Server.Account
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Logic.Enums;

    internal class DisconnectedMessage : Message
    {
        public DisconnectedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 25892;
            }
        }

        internal override void Process()
        {
            this.Device.State = State.DISCONNECTED;
        }
    }
}