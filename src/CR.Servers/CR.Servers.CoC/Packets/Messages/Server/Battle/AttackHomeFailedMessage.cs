namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using CR.Servers.CoC.Logic;

    internal class AttackHomeFailedMessage : Message
    {
        public AttackHomeFailedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24103;
            }
        }
    }
}