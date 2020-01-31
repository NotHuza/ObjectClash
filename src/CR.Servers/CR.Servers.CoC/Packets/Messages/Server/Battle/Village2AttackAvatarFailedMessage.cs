namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class Village2AttackAvatarFailedMessage : Message
    {
        internal int Reason;

        public Village2AttackAvatarFailedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 25022;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(this.Reason);
        }
    }
}