namespace CR.Servers.CoC.Packets.Messages.Client.Home
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using CR.Servers.Extensions.Binary;

    internal class SendGlobalChatLineMessage : Message
    {
        internal string Message;

        public SendGlobalChatLineMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14715;
            }
        }

        internal override void Decode()
        {
            this.Message = this.Reader.ReadString();
        }

        internal override void Process()
        {
            if (this.Device.Chat != null)
            {
                //if (DateTime.UtcNow != this.Device.LastGlobalChatEntry)
                {
                    if (!string.IsNullOrEmpty(this.Message))
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
                                if (this.Message.StartsWith(Factory.Delimiter.ToString()))
                                {
                                    string CommandName;
                                    Debug Debug = Factory.CreateDebug(this.Message, this.Device, out CommandName);

                                    new GlobalChatLineMessage(this.Device, this.Device.GameMode.Level.Player) {Message = this.Message}.Send();

                                    if (Debug != null)
                                    {
                                        if (this.Device.GameMode.Level.Player.Rank >= Debug.RequiredRank)
                                        {
                                            Debug.Process();
                                        }
                                        else
                                        {
                                            Debug.SendChatMessage("Debug command failed. Insufficient privileges.");
                                        }

                                        Debug.Dispose();
                                    }
                                    else
                                    {
                                        this.SendChatMessage($"Unknown command '{CommandName}'. \n~ Available Server Commands ~ \n" +
                "/help (lists available server commands) \n" +
                "/status (prints the server's status) \n" +
                "/id (prints your Account ID) \n" +
                "/addunits (adds 500 of all units) \n" +
                "/clearunits (clears all of your units) \n" +
                "/addspells (adds 500 of all spells) \n" +
                "/clearspells (clears all of your spells) \n" +
                "/ownbaseattack (attack yourself) \n" +
                "/clearobstacles (clears all of your existing obstacles) \n" +
                "/resetbase (clears all of your existing buildings and resets your main base) \n" +
                "/maxlevels (maximize your existing buildings' level) \n" +
                "/setlevel <level> (sets your avatar XP level (example: '/setlevel 500') ) \n" +
                "/setscore <amount> (sets your trophy count (example: '/setscore 5000') ) \n" +
                "/maxresources (refills your storages) \n" +
                "/setbases (set your bases maxed - by inputting the desired TH level. Example: '/setbases 11' - will give you TH11 & BH8 maxed base. You can use TH levels from 1 to 11, as much times as you want.)");
                                    }

                                    return;
                                }

                                this.Device.Chat.AddEntry(this.Device, this.Message);
                            }
                        }
                    }
                }
            }
        }
    }
}