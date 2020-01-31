namespace CR.Servers.CoC.Packets.Messages.Server.Friend
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.Extensions.List;

    internal class FriendRequestFailedMessage : Message
    {
        internal AddFriendErrorReason Reason;

        public FriendRequestFailedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 20112;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt((int) this.Reason);
        }
    }
}