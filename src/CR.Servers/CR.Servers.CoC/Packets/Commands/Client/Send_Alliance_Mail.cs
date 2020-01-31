namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.Extensions.Binary;

    internal class Send_Alliance_Mail : Command
    {
        internal string Message;

        public Send_Alliance_Mail(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 537;
            }
        }

        internal override void Decode()
        {
            this.Message = this.Reader.ReadString();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            if (!Level.GameObjectManager.Bunker.Locked)
            {
                BunkerComponent BunkerComponent = Level.GameObjectManager.Bunker.BunkerComponent;
                if (BunkerComponent != null)
                {
                    if (BunkerComponent.CanSendClanMail)
                    {
                        if (!string.IsNullOrWhiteSpace(this.Message))
                        {
                            if (this.Message.Length <= 256)
                            {
                                this.Message = Resources.Regex.Replace(this.Message, " ");

                                if (this.Message.StartsWith(" "))
                                {
                                    this.Message = this.Message.Remove(0, 1);
                                }

                                if (this.Message.Length > 0)
                                {
                                    foreach (Member Member in Level.Player.Alliance.Members.Slots.Values.ToArray())
                                    {
                                        Member.Player.Inbox.Add(
                                            new AllianceMailAvatarStreamEntry(Level.Player, Level.Player.Alliance)
                                            {
                                                Message = this.Message
                                            });
                                    }
                                }

                                BunkerComponent.ClanMailTimer = new Timer();
                                BunkerComponent.ClanMailTimer.StartTimer(Level.Player.LastTick, Globals.ClanMailCooldown);
                            }
                        }
                    }
                }
            }
        }
    }
}