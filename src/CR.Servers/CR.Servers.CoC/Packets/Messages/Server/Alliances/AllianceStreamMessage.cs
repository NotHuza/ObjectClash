namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.Extensions.List;

    internal class AllianceStreamMessage : Message
    {
        internal Alliance Alliance;

        public AllianceStreamMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24311;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(0);
            this.Alliance.Streams.Encode(this.Data, this.Device.GameMode.Level.Player.UserId);
        }
    }
}