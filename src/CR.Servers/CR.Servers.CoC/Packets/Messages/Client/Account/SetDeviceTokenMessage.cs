namespace CR.Servers.CoC.Packets.Messages.Client.Account
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class SetDeviceTokenMessage : Message
    {
        internal string Password;

        public SetDeviceTokenMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 10113;
            }
        }

        internal override void Decode()
        {
            this.Password = this.Reader.ReadString();
            this.Reader.ReadInt32();
        }
    }
}