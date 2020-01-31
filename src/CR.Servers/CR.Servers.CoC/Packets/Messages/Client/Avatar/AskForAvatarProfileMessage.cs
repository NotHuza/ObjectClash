namespace CR.Servers.CoC.Packets.Messages.Client.Avatar
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Avatar;
    using CR.Servers.Extensions.Binary;
    using System.Threading.Tasks;

    internal class AskForAvatarProfileMessage : Message
    {
        internal int AvatarHighId;
        internal int AvatarLowId;

        public AskForAvatarProfileMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14325;
            }
        }

        internal override void Decode()
        {
            this.AvatarHighId = this.Reader.ReadInt32();
            this.AvatarLowId = this.Reader.ReadInt32();

            if (this.Reader.ReadBooleanV2())
            {
                this.Reader.ReadInt32(); //HomeHighId
                this.Reader.ReadInt32(); //HomeLowId
            }

            this.Reader.ReadBooleanV2();
        }

        internal override async Task ProcessAsync()
        {
            if (this.AvatarHighId >= 0 && this.AvatarLowId > 0)
            {
                Player Player = (await Resources.Accounts.LoadAccountAsync(this.AvatarHighId, this.AvatarLowId))?.Player;

                if (Player?.Level != null)
                {
                    new AvatarProfileMessage(this.Device, Player.Level).Send();
                }
            }
        }
    }
}