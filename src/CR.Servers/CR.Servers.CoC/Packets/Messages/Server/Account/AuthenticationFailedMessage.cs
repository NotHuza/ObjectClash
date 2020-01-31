namespace CR.Servers.CoC.Packets.Messages.Server.Account
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.Extensions.List;

    internal class AuthenticationFailedMessage : Message
    {
        internal string Message;

        internal LoginFailedReason Reason;
        internal string RedirectDomain;

        public AuthenticationFailedMessage(Device Device, LoginFailedReason Reason = LoginFailedReason.Default) : base(Device)
        {
            this.Reason = Reason;
            this.Version = 9;
        }

        internal override short Type
        {
            get
            {
                return 20103;
            }
        }

        internal string PatchingHost
        {
            get
            {
                return Fingerprint.Custom ? Settings.PatchServer : "https://www.clashersrepublic.com/game-content/projectmagic/";
            }
        }

        internal override void Encode()
        {
            this.Data.AddInt((int) this.Reason);
            this.Data.AddString(null);
            this.Data.AddString(this.RedirectDomain);
            this.Data.AddString(null); //Old Patching Host
            this.Data.AddString(Settings.UpdateServer);
            this.Data.AddString(this.Message);
            this.Data.AddInt(this.Reason == LoginFailedReason.Maintenance ? 0 : 1); // Remaining time
            this.Data.AddByte(0);
            this.Data.AddCompressed(this.Reason == LoginFailedReason.Patch ? Fingerprint.Json : null, false);
            this.Data.AddInt(2);
            this.Data.AddString(Settings.GameAssetsServer);
            this.Data.AddString(this.PatchingHost);
            this.Data.AddInt(0);
            this.Data.AddInt(0);
            this.Data.AddInt(-1);
            this.Data.AddInt(-1);
        }
    }
}