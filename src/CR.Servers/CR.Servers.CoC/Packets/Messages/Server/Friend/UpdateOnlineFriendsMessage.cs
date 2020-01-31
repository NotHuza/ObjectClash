namespace CR.Servers.CoC.Packets.Messages.Server.Friend
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class UpdateOnlineFriendsMessage : Message
    {
        internal Friend Friend;

        public UpdateOnlineFriendsMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 20206;
            }
        }

        internal override void Encode()
        {
            this.Data.AddLong(this.Friend.PlayerId);
            this.Data.AddVInt((int) this.Friend.GameState);
            //1 = Player online
        }
    }
}