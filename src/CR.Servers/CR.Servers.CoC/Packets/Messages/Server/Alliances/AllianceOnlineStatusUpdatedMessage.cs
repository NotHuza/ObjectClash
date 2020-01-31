namespace CR.Servers.CoC.Packets.Messages.Server.Alliances
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class AllianceOnlineStatusUpdatedMessage : Message
    {
        internal int Connected;
        internal int TotalMember;

        public AllianceOnlineStatusUpdatedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 20207;
            }
        }

        internal override void Encode()
        {
            this.Data.AddVInt(this.Connected);
            this.Data.AddVInt(this.TotalMember);
        }
    }
}