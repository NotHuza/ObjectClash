namespace CR.Servers.CoC.Packets.Messages.Server.Friend
{
    using CR.Servers.CoC.Logic;

    internal class FriendListUpdateMessage : Message
    {
        internal Friend Friend;

        internal FriendListUpdateMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 20106;
            }
        }

        internal override void Encode()
        {
            this.Friend.Encode(this.Data);
        }
    }
}