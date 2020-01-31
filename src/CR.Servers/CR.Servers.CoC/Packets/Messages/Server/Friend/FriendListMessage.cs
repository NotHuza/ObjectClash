namespace CR.Servers.CoC.Packets.Messages.Server.Friend
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.Extensions.List;

    internal class FriendListMessage : Message
    {
        internal FriendListType ListType;

        public FriendListMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 20105;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt((int) this.ListType); //0 Normal, 1 Facebook, 2 Gamecenter, 3 Tencent
            this.Device.GameMode.Level.Player.Friends.Encode(this.Data);
        }
    }
}