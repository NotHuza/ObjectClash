namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;

    internal class AllianceDataMessage : Message
    {
        internal Alliance Alliance;

        public AllianceDataMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24301;
            }
        }

        internal override void Encode()
        {
            this.Alliance.Encode(this.Data);
        }
    }
}