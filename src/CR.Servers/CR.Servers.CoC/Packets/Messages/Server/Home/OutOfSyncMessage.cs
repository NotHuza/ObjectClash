namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Logic.Enums;

    internal class OutOfSyncMessage : Message
    {
        public OutOfSyncMessage(Device Device) : base(Device)
        {
            // Out_Of_Sync_Message.
        }

        internal override short Type
        {
            get
            {
                return 24104;
            }
        }

        internal override void Process()
        {
            this.Device.State = State.DISCONNECTED;
        }
    }
}