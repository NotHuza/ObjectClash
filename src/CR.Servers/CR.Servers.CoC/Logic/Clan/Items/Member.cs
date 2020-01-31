namespace CR.Servers.CoC.Logic.Clan.Items
{
    using System;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic.Enums;
    using Newtonsoft.Json;

    internal class Member
    {
        private Player _Player;
        [JsonProperty] internal int HighId;
        [JsonProperty] internal DateTime Joined;
        [JsonProperty] internal int LowId;

        [JsonProperty] internal Role Role;

        [JsonProperty] internal int TroopReceived;
        [JsonProperty] internal int TroopSended;

        internal Member()
        {
        }

        internal Member(Player Player)
        {
            this.HighId = Player.HighID;
            this.LowId = Player.LowID;
            this.Player = Player;

            this.Joined = DateTime.UtcNow;
            this.Role = Role.Member;
        }

        internal long PlayerId
        {
            get
            {
                return ((long) this.HighId << 32) | (uint) this.LowId;
            }
        }

        internal int New
        {
            get
            {
                return this.Joined >= DateTime.UtcNow.AddDays(-3) ? 1 : 0;
            }
        }

        internal int TimeSinceJoined
        {
            get
            {
                return (int) DateTime.UtcNow.Subtract(this.Joined).TotalSeconds;
            }
        }

        internal Player Player
        {
            get
            {
                if (this._Player != null)
                {
                    return this._Player;
                }

                this._Player = Resources.Accounts.LoadAccountAsync(this.HighId, this.LowId).Result?.Player;

                if (this._Player == null)
                {
                    Logging.Error(this.GetType(), $"LoadLevel returned null for Player with account id {this.HighId} - {this.LowId}");
                }

                return this._Player;
            }
            set
            {
                this._Player = value;
            }
        }
    }
}