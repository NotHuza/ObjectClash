namespace CR.Servers.CoC.Logic.Clan
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Clan.Slots;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Alliances;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;

    internal class Alliance
    {
        internal int Connected;

        [JsonProperty] internal string Description;
        [JsonProperty] internal AllianceHeader Header;
        [JsonProperty] internal int HighId;
        [JsonProperty] internal int LowId;

        [JsonProperty] internal Members Members;
        [JsonProperty] internal Streams Streams;
        [JsonProperty] internal WarState WarState;

        internal Alliance()
        {
            this.Header = new AllianceHeader(this);
            this.Members = new Members(this);
            this.Streams = new Streams(this);
        }

        internal Alliance(int HighID, int LowID) : this()
        {
            this.HighId = HighID;
            this.LowId = LowID;
        }

        internal long AllianceId
        {
            get
            {
                return ((long) this.HighId << 32) | (uint) this.LowId;
            }
        }

        internal void Encode(List<byte> Packet)
        {
            this.Header.Encode(Packet);

            Packet.AddString(this.Description);
            Packet.AddInt(0);
            Packet.AddBool(false);
            Packet.AddInt(0);
            Packet.AddBool(false);

            this.Members.Encode(Packet);

            Packet.AddInt(0);
            Packet.AddInt(52);
        }

        internal void IncrementTotalConnected()
        {
            if (this.Connected > this.Header.NumberOfMembers)
            {
                Logging.Error(this.GetType(), "Tried to increase online alliance members beyond alliance total member.");
            }
            else
            {
                int Connected = Interlocked.Increment(ref this.Connected);

                foreach (Member Member in this.Members.Slots.Values.ToList())
                {
                    Player Player = Member.Player;

                    if (Player != null && Player.Connected)
                    {
                        new AllianceOnlineStatusUpdatedMessage(Player.Level.GameMode.Device)
                        {
                            Connected = Connected,
                            TotalMember = this.Header.NumberOfMembers
                        }.Send();
                    }
                }
            }
        }

        internal void DecrementTotalConnected()
        {
            if (this.Connected == 0)
            {
                Logging.Error(this.GetType(), "Tried to decrease online alliance members beyond 0.");
            }
            else
            {
                int Connected = Interlocked.Decrement(ref this.Connected);

                foreach (Member Member in this.Members.Slots.Values.ToList())
                {
                    Player Player = Member.Player;

                    if (Player != null && Player.Connected)
                    {
                        new AllianceOnlineStatusUpdatedMessage(Player.Level.GameMode.Device)
                        {
                            Connected = Connected,
                            TotalMember = this.Header.NumberOfMembers
                        }.Send();
                    }
                }
            }
        }

        internal void RefreshTotalConnected()
        {
            foreach (Member Member in this.Members.Slots.Values.ToList())
            {
                Player Player = Member.Player;

                if (Player != null && Player.Connected)
                {
                    new AllianceOnlineStatusUpdatedMessage(Player.Level.GameMode.Device)
                    {
                        Connected = this.Connected,
                        TotalMember = this.Header.NumberOfMembers
                    }.Send();
                }
            }
        }


        public override string ToString()
        {
            return this.HighId + "-" + this.LowId;
        }
    }
}