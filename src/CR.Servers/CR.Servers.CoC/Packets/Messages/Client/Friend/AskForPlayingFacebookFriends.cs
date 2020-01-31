namespace CR.Servers.CoC.Packets.Messages.Client.Friend
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class AskForPlayingFacebookFriends : Message
    {
        public AskForPlayingFacebookFriends(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 10513;
            }
        }

        internal override void Process()
        {
            //new Friend_List(this.Device) { ListType = FriendListType.Facebook}.Send();
        }
    }
}