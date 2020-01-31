namespace CR.Servers.CoC.Packets.Messages.Server.Avatar
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class Village2AttackTargetsDataMessage : Message
    {
        public Village2AttackTargetsDataMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24370;
            }
        }

        internal override void Encode()
        {
            this.Data.AddBool(false);
            this.Data.AddInt(100);

            for (int i = 0; i < 100; i++)
            {
                this.Data.AddInt(123456789);
            }
        }
    }
}