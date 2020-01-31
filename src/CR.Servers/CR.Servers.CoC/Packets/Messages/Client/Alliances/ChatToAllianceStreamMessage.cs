namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.Extensions.Binary;

    internal class ChatToAllianceStreamMessage : Message
    {
        internal string Message;

        public ChatToAllianceStreamMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14315;
            }
        }

        internal override void Decode()
        {
            this.Message = this.Reader.ReadString();
        }

        internal override void Process()
        {
            Level level = this.Device.GameMode.Level;

            if (level.Player.InAlliance)
            {
                if (!string.IsNullOrWhiteSpace(this.Message))
                {
                    if (this.Message.Length <= 128)
                    {
                        this.Message = Resources.Regex.Replace(this.Message, " ");

                        if (this.Message.StartsWith(" "))
                        {
                            this.Message = this.Message.Remove(0, 1);
                        }

                        if (this.Message.Length > 0)
                        {
                            level.Player.Alliance.Streams.AddEntry(
                                new ChatStreamEntry(this.Device.GameMode.Level.Player.AllianceMember)
                                {
                                    Message = this.Message
                                });
                        }
                    }
                }
            }
        }
    }
}