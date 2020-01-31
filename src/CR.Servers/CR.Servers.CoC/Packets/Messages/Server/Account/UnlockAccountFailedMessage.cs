namespace CR.Servers.CoC.Packets.Messages.Server.Account
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.Extensions.List;

    internal class UnlockAccountFailedMessage : Message
    {
        internal UnlockAccountReason Reason;

        internal UnlockAccountFailedMessage(Device device) : base(device)
        {
            this.Version = 1;
        }

        internal override short Type
        {
            get
            {
                return 20133;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt((int) this.Reason);
        }
    }
}