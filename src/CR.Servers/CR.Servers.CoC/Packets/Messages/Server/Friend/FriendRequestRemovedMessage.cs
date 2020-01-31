namespace CR.Servers.CoC.Packets.Messages.Server.Friend
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class FriendRequestRemovedMessage : Message
    {
        public FriendRequestRemovedMessage(Device Device) : base(Device)
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
            this.Data.AddInt(1);
        }
    }
}