namespace CR.Servers.CoC.Packets.Messages.Server.Avatar
{
    using CR.Servers.CoC.Logic;

    internal class AvatarStreamEntryMessage : Message
    {
        internal AvatarStreamEntry StreamEntry;

        public AvatarStreamEntryMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24412;
            }
        }

        internal override void Encode()
        {
            this.StreamEntry.Encode(this.Data);
        }
    }
}