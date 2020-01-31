namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.CoC.Packets.Messages.Server.Alliances;
    using CR.Servers.Extensions.Binary;
    using System.Threading.Tasks;

    internal class JoinAllianceMessage : Message
    {
        internal int ClanHighId;
        internal int ClanLowId;

        public JoinAllianceMessage(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14305;
            }
        }

        internal override void Decode()
        {
            this.ClanHighId = this.Reader.ReadInt32();
            this.ClanLowId = this.Reader.ReadInt32();
        }

        internal override async Task ProcessAsync()
        {
            Alliance Alliance = await Resources.Clans.GetAsync(this.ClanHighId, this.ClanLowId);

            if (Alliance != null)
            {
                Player Avatar = this.Device.GameMode.Level.Player;

                foreach (AvatarStreamEntry entry in Avatar.Inbox.Entries.Values.Where(T => T.Type == AvatarStream.Invitation).ToArray())
                {
                    Avatar.Inbox.Remove(entry);
                }

                Member member;
                if (Alliance.Members.Join(Avatar, out member))
                {
                    Avatar.SetAlliance(Alliance, member);
                    Avatar.AllianceHighId = Alliance.HighId;
                    Avatar.AllianceLowId = Alliance.LowId;

                    new AllianceDataMessage(this.Device)
                    {
                        Alliance = Alliance
                    }.Send();
                    new AllianceStreamMessage(this.Device)
                    {
                        Alliance = Alliance
                    }.Send();

                    Alliance.IncrementTotalConnected();
                    Alliance.Streams.AddEntry(new EventStreamEntry(member, member, AllianceEvent.Joined));

                    this.Device.GameMode.CommandManager.AddCommand(
                        new Joined_Alliance(this.Device)
                        {
                            AllianceId = Alliance.AllianceId,
                            AllianceName = Alliance.Header.Name,
                            AllianceBadge = Alliance.Header.Badge,
                            AllianceLevel = Alliance.Header.ExpLevel,
                            CreateAlliance = false
                        }
                    );
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to join the clan, Join() function returned false");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to join the clan. Get() returned a null clan");
            }
        }
    }
}