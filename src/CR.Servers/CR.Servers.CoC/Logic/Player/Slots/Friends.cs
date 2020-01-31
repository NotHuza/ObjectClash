namespace CR.Servers.CoC.Logic
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using CR.Servers.CoC.Core;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;

    internal class Friends
    {
        internal ConcurrentDictionary<long, Player> Connected;
        internal Player Player;

        [JsonProperty] internal ConcurrentDictionary<long, Friend> Slots;

        public Friends()
        {
            this.Slots = new ConcurrentDictionary<long, Friend>();
            this.Connected = new ConcurrentDictionary<long, Player>();
        }

        public Friends(Player Player) : this()
        {
            this.Player = Player;
        }

        internal bool Add(Player Player, out Friend Friend)
        {
            Friend = new Friend(Player);

            if (!this.Slots.ContainsKey(Player.UserId))
            {
                if (this.Slots.TryAdd(Player.UserId, Friend))
                {
                    if (Player.Connected)
                    {
                        this.Connected.TryAdd(Player.UserId, Player);
                    }

                    return true;
                }
            }
            else
            {
                Friend.State = this.Slots[Player.UserId].State;
                this.Slots[Player.UserId] = Friend;

                if (Player.Connected)
                {
                    this.Connected.TryAdd(Player.UserId, Player);
                }

                return true;
            }

            return false;
        }

        internal async void VerifyFriend([CallerMemberName] string callerName = "")
        {
            if (this.Slots != null)
            {
                foreach (Friend Friend in this.Slots.Values.ToArray())
                {
                    if (Friend.Player == null)
                    {
                        Account Account = await Resources.Accounts.LoadAccountAsync(Friend.HighId, Friend.LowId);
                        
                        if (Account.Player != null)
                        {
                            Friend.Player = Account.Player;
                        }
                        else
                        {
                            Friend _;
                            this.Remove(Friend.PlayerId, out _);
                        }
                    }
                }
            }
        }

        internal bool Remove(long Player, out Friend Friend)
        {
            if (this.Slots.TryRemove(Player, out Friend))
            {
                Player _;
                this.Connected.TryRemove(Player, out _);
                return true;
            }

            return false;
        }

        internal bool Remove(Player Player, out Friend Friend)
        {
            return this.Remove(Player.UserId, out Friend);
        }

        internal bool Remove(Friend Player, out Friend Friend)
        {
            return this.Remove(Player.PlayerId, out Friend);
        }

        internal Friend Get(long UserID)
        {
            return this.Slots.ContainsKey(UserID) ? this.Slots[UserID] : null;
        }

        internal void Encode(List<byte> Packet)
        {
            Friend[] Friends = this.Slots.Values.ToArray();

            Packet.AddInt(Friends.Length);
            foreach (Friend Friend in Friends)
            {
                Friend.Encode(Packet);
            }
        }
    }
}