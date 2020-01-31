namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;

    internal class Friend
    {
        internal FriendGameState GameState = FriendGameState.Disconnected;
        [JsonProperty] internal int HighId;
        [JsonProperty] internal int LowId;
        internal bool Remove;
        [JsonProperty] internal FriendState State;

        internal Friend()
        {
        }

        internal Friend(Player Player)
        {
            this.HighId = Player.HighID;
            this.LowId = Player.LowID;
            this.Player = Player;
        }

        internal long PlayerId
        {
            get
            {
                return ((long) this.HighId << 32) | (uint) this.LowId;
            }
        }

        internal Player Player { get; set; }

        internal void Encode(List<byte> Packet)
        {
            Packet.AddLong(this.PlayerId);
            Packet.AddBool(true);
            Packet.AddLong(this.PlayerId);
            Packet.AddString(this.Player.Name);
            Packet.AddString(this.Player.Facebook.Identifier == string.Empty ? null : this.Player.Facebook.Identifier);
            Packet.AddString(null);
            Packet.AddString(null);
            Packet.AddInt(0); //Protection Time
            Packet.AddInt(this.Player.ExpLevel);
            Packet.AddInt(this.Player.League);
            Packet.AddInt(this.Player.Score);
            Packet.AddInt(this.Player.DuelScore);
            Packet.AddInt((int) this.State); //2 = Request Send, 3 = Want to be friend, 4 = friend 
            Packet.AddInt(0);

            if (this.Player.InAlliance)
            {
                Packet.AddBool(true);
                Packet.AddLong(this.Player.AllianceId);
                Packet.AddInt(this.Player.Alliance.Header.Badge);
                Packet.AddString(this.Player.Alliance.Header.Name);
                Packet.AddInt((int) this.Player.AllianceMember.Role);
                Packet.AddInt(this.Player.Alliance.Header.ExpLevel);
            }

            Packet.AddBool(false);
        }
    }
}