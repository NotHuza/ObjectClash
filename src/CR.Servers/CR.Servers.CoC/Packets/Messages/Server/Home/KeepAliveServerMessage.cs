namespace CR.Servers.CoC.Packets.Messages.Server.Home
{
    using CR.Servers.CoC.Logic;

    internal class KeepAliveServerMessage : Message
    {
        public KeepAliveServerMessage(Device device) : base(device)
        {
        }

        public override MessagePriority Priority
        {
            get
            {
                return MessagePriority.High;
            }
        }

        internal override short Type
        {
            get
            {
                return 20108;
            }
        }
    }
}