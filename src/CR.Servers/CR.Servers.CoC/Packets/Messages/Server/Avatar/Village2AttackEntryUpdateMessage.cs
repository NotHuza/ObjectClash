namespace CR.Servers.CoC.Packets.Messages.Server.Avatar
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Duel.Entry;

    using CR.Servers.Extensions.List;

    internal class Village2AttackEntryUpdateMessage : Message
    {
        internal Village2AttackEntry Entry;

        public Village2AttackEntryUpdateMessage(Device Device, Village2AttackEntry entry) : base(Device)
        {
            this.Entry = entry;
        }

        internal override short Type
        {
            get
            {
                return 24371;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(this.Entry.GetEntryType());
            this.Entry.Encode(this.Data);
        }
    }
}