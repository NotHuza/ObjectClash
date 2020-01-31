namespace CR.Servers.CoC.Packets.Messages.Server.Api
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.List;

    internal class FacebookAccountBoundMessage : Message
    {
        public FacebookAccountBoundMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 24201;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt(1);
        }
    }
}