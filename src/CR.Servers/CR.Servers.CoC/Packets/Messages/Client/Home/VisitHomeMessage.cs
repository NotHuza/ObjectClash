namespace CR.Servers.CoC.Packets.Messages.Client.Home
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Extensions.Binary;
    using System.Threading.Tasks;

    internal class VisitHomeMessage : Message
    {
        internal int HighId;
        internal int LowId;

        public VisitHomeMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14113;
            }
        }

        internal override void Decode()
        {
            this.HighId = this.Reader.ReadInt32();
            this.LowId = this.Reader.ReadInt32();
        }

        internal override async Task ProcessAsync()
        {
            Player Player = (await Resources.Accounts.LoadAccountAsync(this.HighId, this.LowId)).Player;

            if (Player.Level != null)
            {
                new VisitHomeDataMessage(this.Device, Player.Level).Send();
            }
            else
            {
                new OwnHomeDataMessage(this.Device).Send();
            }
        }
    }
}