namespace CR.Servers.CoC.Packets.Messages.Client.Friend
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Friend;
    using CR.Servers.Extensions.Binary;

    internal class AskForAvatarFriendListMessage : Message
    {
        public AskForAvatarFriendListMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 10504;
            }
        }

        internal override void Process()
        {
            //new FriendListMessage(this.Device).Send();
        }
    }
}