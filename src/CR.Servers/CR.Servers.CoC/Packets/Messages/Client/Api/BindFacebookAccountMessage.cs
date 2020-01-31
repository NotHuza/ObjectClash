namespace CR.Servers.CoC.Packets.Messages.Client.API
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Api;
    using CR.Servers.Extensions.Binary;
    using System.Threading.Tasks;

    internal class BindFacebookAccountMessage : Message
    {
        internal string Identifier;
        internal string Token;

        internal bool Unknown;

        public BindFacebookAccountMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14201;
            }
        }

        internal override void Decode()
        {
            this.Unknown = this.Reader.ReadBoolean();
            this.Identifier = this.Reader.ReadString();
            this.Token = this.Reader.ReadString();
        }

        internal override async Task ProcessAsync()
        {
            Level level = this.Device.GameMode.Level;

            if (!string.IsNullOrEmpty(this.Identifier))
            {
                Player Player = (await Resources.Accounts.LoadAccountViaFacebookAsync(this.Identifier))?.Player;

                if (Player != null)
                {
                    if (Player.UserId != level.Player.UserId)
                    {
                        Player.Facebook.Identifier = string.Empty;
                        Player.Facebook.Token = string.Empty;
                    }
                }

                level.Player.Facebook.Identifier = this.Identifier;
                level.Player.Facebook.Token = this.Token;

                new FacebookAccountBoundMessage(this.Device).Send();
            }
        }
    }
}