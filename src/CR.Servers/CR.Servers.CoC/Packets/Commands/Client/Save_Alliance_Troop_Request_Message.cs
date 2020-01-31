namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Save_Alliance_Troop_Request_Message : Command
    {
        internal string Message;

        public Save_Alliance_Troop_Request_Message(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 540;
            }
        }

        internal override void Decode()
        {
            this.Message = this.Reader.ReadString();
            base.Decode();
        }

        internal override void Execute()
        {
            this.Device.GameMode.Level.TroopRequestMessage = this.Message;
        }
    }
}