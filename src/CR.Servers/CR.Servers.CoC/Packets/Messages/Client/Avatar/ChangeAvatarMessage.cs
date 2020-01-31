namespace CR.Servers.CoC.Packets.Messages.Client.Avatar
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.CoC.Packets.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Avatar;
    using CR.Servers.Extensions.Binary;

    internal class ChangeAvatarMessage : Message
    {
        internal string AvatarName;
        internal bool NameSetByUser;

        public ChangeAvatarMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 10212;
            }
        }

        internal override void Decode()
        {
            this.AvatarName = this.Reader.ReadString();
            this.NameSetByUser = this.Reader.ReadBoolean();
        }

        internal override void Process()
        {
            if (!this.Device.GameMode.CommandManager.ChangeNameOnGoing)
            {
                if (!string.IsNullOrEmpty(this.AvatarName))
                {
                    if (Resources.Name.IsMatch(this.AvatarName))
                    {
                        if (!this.Device.GameMode.Level.Player.NameSetByUser == this.NameSetByUser)
                        {
                            if (this.AvatarName.Length <= 16)
                            {
                                this.AvatarName = Resources.Regex.Replace(this.AvatarName, " ");

                                if (this.AvatarName.StartsWith(" "))
                                {
                                    this.AvatarName = this.AvatarName.Remove(0, 1);
                                }

                                if (this.AvatarName.Length >= 2)
                                {
                                    this.Device.GameMode.CommandManager.ChangeNameOnGoing = true;
                                    this.Device.GameMode.CommandManager.AddCommand(
                                        new Name_Change_Callback(this.Device)
                                        {
                                            AvatarName = this.AvatarName,
                                            ChangeNameCount = this.Device.GameMode.Level.Player.ChangeNameCount
                                        });
                                }
                                else
                                {
                                    new ChangeAvatarNameFailedMessage(this.Device) {Error = NameErrorReason.NameTooShort}.Send();
                                }
                            }
                            else
                            {
                                new ChangeAvatarNameFailedMessage(this.Device) {Error = NameErrorReason.NameInvalid}.Send();
                            }
                        }
                    }
                    else
                    {
                        new ChangeAvatarNameFailedMessage(this.Device) {Error = NameErrorReason.NameInvalid}.Send();
                    }
                }
            }
        }
    }
}