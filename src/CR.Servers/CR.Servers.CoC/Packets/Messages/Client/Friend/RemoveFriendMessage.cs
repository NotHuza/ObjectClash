namespace CR.Servers.CoC.Packets.Messages.Client.Friend
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Friend;
    using CR.Servers.Extensions.Binary;

    internal class RemoveFriendMessage : Message
    {
        internal long UserId;

        public RemoveFriendMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 10506;
            }
        }

        internal override void Decode()
        {
            this.UserId = this.Reader.ReadInt64();
        }

        internal override void Process()
        {
            /*Level Level = this.Device.GameMode.Level;
            Friend Friend = Level.Player.Friends.Get(this.UserId);

            if (Friend != null)
            {
                Player Player = Friend.Player;

                if (Player != null)
                {
                    if (Level.Player.Friends.Remove(Friend, out _))
                    {
                        if (Player.Friends.Remove(Level.Player, out Friend LevelFriend))
                        {
                            Friend.State = FriendState.Removed;
                            new FriendListUpdateMessage(this.Device) {Friend = Friend}.Send();

                            if (Player.Connected)
                            {
                                LevelFriend.State = FriendState.Removed;
                                new FriendListUpdateMessage(Player.Level.GameMode.Device) {Friend = LevelFriend}
                                    .Send();
                            }
                        }
                        else
                        {
                            Logging.Error(this.GetType(), $"Unexpected issue while removing friend. Player Remove() function returned false!");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), $"Unexpected issue while removing friend. Level Remove() function returned false!");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), $"Unexpected issue while removing friend. Player is null!");
                }
            }
            else
            {
                Logging.Error(this.GetType(), $"Unexpected issue while removing friend. Friend is null!");
            }*/
        }
    }
}