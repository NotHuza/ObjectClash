namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.CoC.Packets.Messages.Server.Alliances;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Logic.Enums;
    using System.Threading.Tasks;

    internal class ChangeAllianceSettingsMessage : Message
    {
        internal int AllianceBadge;

        internal Hiring AllianceType;
        internal bool AmicalWar;

        internal string Description;
        internal int Origin;

        internal bool PublicWarLog;
        internal int RequiredDuelScore;
        internal int RequiredScore;
        internal int WarFrequency;

        public ChangeAllianceSettingsMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14316;
            }
        }

        internal override void Decode()
        {
            this.Description = this.Reader.ReadString();
            this.Reader.ReadString();

            this.AllianceBadge = this.Reader.ReadInt32();
            this.AllianceType = (Hiring) this.Reader.ReadInt32();
            this.RequiredScore = this.Reader.ReadInt32();
            this.RequiredDuelScore = this.Reader.ReadInt32();
            this.WarFrequency = this.Reader.ReadInt32();

            this.Origin = this.Reader.ReadInt32();

            this.PublicWarLog = this.Reader.ReadBooleanV2();
            this.AmicalWar = this.Reader.ReadBooleanV2();
        }

        internal override async Task ProcessAsync()
        {
            if (this.Device.GameMode.Level.Player.InAlliance)
            {
                if (this.Description != null)
                {
                    this.Description = Resources.Regex.Replace(this.Description, " ");

                    if (this.Description.StartsWith(" "))
                    {
                        this.Description = this.Description.Remove(0, 1);
                    }

                    if (this.Description.Length > 128)
                    {
                        this.Description = this.Description.Substring(0, 128);
                    }
                }
                else
                {
                    new ChangeAllianceSettingsFailedMessage(this.Device).Send();
                    return;
                }

                //AllianceBadgeLayerData Background = (AllianceBadgeLayerData)CSV.Tables.Get(Gamefile.AllianceBadgeLayer).GetDataWithInstanceID(this.AllianceBadge % 0x100);
                //AllianceBadgeLayerData Middle = (AllianceBadgeLayerData)CSV.Tables.Get(Gamefile.AllianceBadgeLayer).GetDataWithInstanceID(this.AllianceBadge % 0x1000000 / 0x100);
                //AllianceBadgeLayerData Foreground = (AllianceBadgeLayerData)CSV.Tables.Get(Gamefile.AllianceBadgeLayer).GetDataWithInstanceID(this.AllianceBadge / 0x1000000);

                //if (Background != null)
                {
                    //if (Middle != null)
                    {
                        //if (Foreground != null)
                        {
                            Level Level = this.Device.GameMode.Level;
                            Alliance Alliance = await Resources.Clans.GetAsync(Level.Player.AllianceHighId, Level.Player.AllianceLowId);
                            Alliance.Description = this.Description;
                            Alliance.Header.Badge = this.AllianceBadge;
                            Alliance.Header.Type = this.AllianceType;
                            Alliance.Header.WarFrequency = this.WarFrequency;
                            Alliance.Header.Origin = this.Origin;
                            Alliance.Header.PublicWarLog = this.PublicWarLog;
                            Alliance.Header.RequiredScore = this.RequiredScore;
                            Alliance.Header.RequiredDuelScore = this.RequiredDuelScore;
                            Alliance.Header.AmicalWar = this.AmicalWar;

                            Alliance.Streams.AddEntry(new EventStreamEntry(Level.Player.AllianceMember,
                                Level.Player.AllianceMember, AllianceEvent.ChangedSettings));

                            foreach (Player Player in Alliance.Members.Connected.Values.ToArray())
                            {
                                if (Player.Connected)
                                {
                                    Player.Level.GameMode.CommandManager.AddCommand(
                                        new Changed_Alliance_Settings(Player.Level.GameMode.Device)
                                        {
                                            AllianceId = Alliance.AllianceId,
                                            AllianceBadge = Alliance.Header.Badge
                                        });
                                }
                                else
                                {
                                    Player _;
                                    Alliance.Members.Connected.TryRemove(Player.UserId, out _);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}