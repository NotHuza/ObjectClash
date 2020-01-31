namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class AllianceStreamEntryRemovedMessage : Message
    {
        internal long MessageId;

        public AllianceStreamEntryRemovedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24318;
            }
        }

        internal override void Encode()
        {
            this.Data.AddLong(this.MessageId);
        }
    }
}