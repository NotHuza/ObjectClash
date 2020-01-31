namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class League_Notifications_Seen : Command
    {
        internal int League;
        internal int Unknown;

        public League_Notifications_Seen(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 538;
            }
        }

        internal override void Decode()
        {
            this.League = this.Reader.ReadInt32();
            this.Unknown = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;

            Level.LastLeagueRank = this.League;
            Level.LastLeagueShuffleInfo = 0;
        }
    }
}