namespace CR.Servers.CoC.Packets.Messages.Server.Avatar
{
    using CR.Servers.CoC.Logic;

    internal class Village2AttackEntryRemovedMessage : Message
    {
        public Village2AttackEntryRemovedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24373;
            }
        }
    }
}