namespace CR.Servers.CoC.Packets.Messages.Server.Avatar
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.Extensions.List;

    internal class ChangeAvatarNameFailedMessage : Message
    {
        internal NameErrorReason Error;

        public ChangeAvatarNameFailedMessage(Device Device) : base(Device)
        {
        }

        internal override short Type
        {
            get
            {
                return 20205;
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt((int) this.Error);
        }
    }
}