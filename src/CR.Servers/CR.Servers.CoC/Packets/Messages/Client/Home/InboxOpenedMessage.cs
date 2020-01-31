namespace CR.Servers.CoC.Packets.Messages.Client.Home
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class InboxOpenedMessage : Message
    {
        public InboxOpenedMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 10905;
            }
        }

        internal override void Decode()
        {
            this.Reader.ReadVInt();
        }
    }
}