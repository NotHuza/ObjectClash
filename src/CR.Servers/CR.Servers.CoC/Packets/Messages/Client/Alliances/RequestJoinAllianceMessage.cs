namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.Extensions.Binary;
    using System.Threading.Tasks;

    internal class RequestJoinAllianceMessage : Message
    {
        internal int HighId;
        internal int LowId;
        internal string Message;

        public RequestJoinAllianceMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14317;
            }
        }

        internal override void Decode()
        {
            this.HighId = this.Reader.ReadInt32();
            this.LowId = this.Reader.ReadInt32();
            this.Message = this.Reader.ReadString();
        }

        internal override async Task ProcessAsync()
        {
            Player Player = this.Device.GameMode.Level.Player;
            Alliance Alliance = await Resources.Clans.GetAsync(this.HighId, this.LowId);

            if (Alliance != null)
            {
                Alliance.Streams.AddEntry(new JoinRequestStreamEntry
                {
                    SenderHighId = Player.HighID,
                    SenderLowId = Player.LowID,
                    SenderLeague = Player.League,
                    SenderName = Player.Name,
                    SenderLevel = Player.ExpLevel,
                    SenderRole = 0
                });
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to request to join the alliance. The alliance is null");
            }
        }
    }
}