namespace CR.Servers.CoC.Packets.Messages.Client.Friend
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Friend;
    using CR.Servers.Extensions.Binary;

    internal class FriendRequestMessage : Message
    {
        internal int HighId;
        internal int LowId;

        public FriendRequestMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 10502;
            }
        }

        internal override void Decode()
        {
            this.HighId = this.Reader.ReadInt32();
            this.LowId = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            /*Level Level = this.Device.GameMode.Level;
            Player Player = Resources.Accounts.LoadAccount(this.HighId, this.LowId)?.Player;

            if (Player != null)
            {
                if (Player.UserId != Level.Player.UserId)
                {
                    if (Player.Friends.Add(Level.Player, out Friend Out))
                    {
                        if (Level.Player.Friends.Add(Player, out Friend Send))
                        {
                            Out.State = FriendState.ReceivedFriendRequest;
                            Send.State = FriendState.SendFriendRequest;

                            new FriendListUpdateMessage(this.Device)
                            {
                                Friend = Send
                            }.Send();

                            if (Player.Connected)
                            {
                                new FriendListUpdateMessage(Player.Level.GameMode.Device)
                                {
                                    Friend = Out
                                }.Send();
                            }
                        }
                        else
                        {
                            new FriendRequestFailedMessage(this.Device)
                            {
                                Reason = AddFriendErrorReason.Generic
                            }.Send();

                            Level.Player.Friends.Remove(Player, out _);
                            Player.Friends.Remove(Level.Player, out _);
                        }
                    }
                    else
                    {
                        new FriendRequestFailedMessage(this.Device)
                        {
                            Reason = AddFriendErrorReason.Generic
                        }.Send();

                        Player.Friends.Remove(Level.Player, out _);
                    }
                }
                else
                {
                    new FriendRequestFailedMessage(this.Device)
                    {
                        Reason = AddFriendErrorReason.OwnAvatar
                    }.Send();
                }
            }
            else
            {
                new FriendRequestFailedMessage(this.Device)
                {
                    Reason = AddFriendErrorReason.DoesNotExist
                }.Send();
            }*/
        }
    }
}