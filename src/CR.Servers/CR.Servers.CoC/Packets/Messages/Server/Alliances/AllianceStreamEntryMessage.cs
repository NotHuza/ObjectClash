namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;

    internal class AllianceStreamEntryMessage : Message
    {
        internal StreamEntry StreamEntry;

        public AllianceStreamEntryMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24312;
            }
        }

        internal override void Encode()
        {
            this.StreamEntry.RequesterId = this.Device.GameMode.Level.Player.UserId;
            this.StreamEntry.Encode(this.Data);
        }
    }
}