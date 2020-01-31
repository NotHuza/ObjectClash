namespace CR.Servers.CoC.Packets.Messages.Client.Account
{
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Account;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;

    internal class ClientHelloMessage : Message
    {
        internal int AppStore;
        internal int BuildVersion;

        internal string ContentHash;
        internal int DeviceType;
        internal int KeyVersion;

        internal int MajorVersion;
        internal int MinorVersion;

        internal int Protocol;

        public ClientHelloMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
            Device.State = State.SESSION;
        }

        internal override short Type
        {
            get
            {
                return 10100;
            }
        }

        internal override void Decode()
        {
            this.Protocol = this.Reader.ReadInt32();
            this.KeyVersion = this.Reader.ReadInt32();

            this.MajorVersion = this.Reader.ReadInt32();
            this.MinorVersion = this.Reader.ReadInt32();
            this.BuildVersion = this.Reader.ReadInt32();

            this.ContentHash = this.Reader.ReadString();

            this.DeviceType = this.Reader.ReadInt32();
            this.AppStore = this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            new AuthenticationFailedMessage(this.Device, LoginFailedReason.Patch).Send();
            /*if (this.Protocol == 1)
            {
                if (this.MajorVersion == Version.ClientMajorVersion && this.BuildVersion == Logic.Version.ClientBuildVersion)
                {
                    // if (this.ContentHash.Equals(Fingerprint.Sha))
                    {
                        if (this.Device.PepperState.LoadKey(this.KeyVersion))
                        {
                            new Server_Hello_Message(this.Device).Send();
                        }
                    }
                    /*else
                        new Authentification_Failed_Message(this.Device, Reason.Patch).Send();
                }
                else
                    new Authentification_Failed_Message(this.Device, Reason.Update).Send();*/
        }
    }
}