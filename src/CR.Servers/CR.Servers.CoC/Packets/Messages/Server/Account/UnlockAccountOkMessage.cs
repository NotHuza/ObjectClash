namespace CR.Servers.CoC.Packets.Messages.Server.Account
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class UnlockAccountOkMessage : Message
    {
        internal Player Account;

        internal UnlockAccountOkMessage(Device device) : base(device)
        {
        }

        internal override short Type
        {
            get
            {
                return 20132;
            }
        }

        internal override void Encode()
        {
            this.Data.AddLong(this.Account.UserId);
            this.Data.AddString(this.Account.Token);
        }
    }
}