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

    internal class RespondToAllianceJoinRequestMessage : Message
    {
        internal bool Decision;

        internal long StreamId;

        public RespondToAllianceJoinRequestMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14321;
            }
        }

        internal override void Decode()
        {
            this.StreamId = this.Reader.ReadInt64();
            this.Decision = this.Reader.ReadBoolean();
        }

        internal override void Process()
        {
            Player Player = this.Device.GameMode.Level.Player;
            Alliance Alliance = Player.Alliance;

            if (Alliance != null)
            {
                Member AllianceMember = Player.AllianceMember;
                if (AllianceMember != null)
                {
                    if (AllianceMember.Role != Role.Member)
                    {
                        StreamEntry Stream = Alliance.Streams.Get(this.StreamId);

                        if (Stream != null)
                        {
                            if (Stream is JoinRequestStreamEntry)
                            {
                                JoinRequestStreamEntry JoinRequest = (JoinRequestStreamEntry)Stream;
                                if (this.Decision)
                                {
                                    Player Target = Resources.Accounts.LoadAccount(JoinRequest.SenderHighId, JoinRequest.SenderLowId)?.Player;

                                    if (Target != null)
                                    {
                                        if (Target.AllianceId == 0)
                                        {
                                            foreach (AvatarStreamEntry entry in Target.Inbox.Entries.Values.Where(T => T.Type == AvatarStream.Invitation).ToArray())
                                            {
                                                Target.Inbox.Remove(entry);
                                            }

                                            Member member;
                                            if (Alliance.Members.Join(Target, out member))
                                            {
                                                Target.SetAlliance(Alliance, member);
                                                Target.AllianceHighId = Alliance.HighId;
                                                Target.AllianceLowId = Alliance.LowId;

                                                if (Target.Connected)
                                                {
                                                    new AllianceDataMessage(this.Device)
                                                    {
                                                        Alliance = Alliance
                                                    }.Send();
                                                    new AllianceStreamMessage(this.Device)
                                                    {
                                                        Alliance = Alliance
                                                    }.Send();

                                                    Target.Level.GameMode.CommandManager.AddCommand(
                                                        new Joined_Alliance(this.Device)
                                                        {
                                                            AllianceId = Alliance.AllianceId,
                                                            AllianceName = Alliance.Header.Name,
                                                            AllianceBadge = Alliance.Header.Badge,
                                                            AllianceLevel = Alliance.Header.ExpLevel,
                                                            CreateAlliance = false
                                                        }
                                                    );

                                                    Alliance.IncrementTotalConnected();
                                                }
                                                else
                                                {
                                                    Alliance.RefreshTotalConnected();
                                                }

                                                Alliance.Streams.AddEntry(new EventStreamEntry(member, AllianceMember, AllianceEvent.Accepted));
                                            }
                                            else
                                            {
                                                Logging.Error(this.GetType(), "Unable to respond to alliance join request. Join() function returned false!");
                                            }
                                        }
                                        else
                                        {
                                            Alliance.Streams.RemoveEntry(JoinRequest);
                                        }
                                    }
                                    else
                                    {
                                        Logging.Error(this.GetType(), "Unable to respond to alliance join request. The target player is null!");
                                    }
                                }

                                JoinRequest.Judge = Player.Name;
                                JoinRequest.State = this.Decision ? InviteState.Accepted : InviteState.Refused;
                                Alliance.Streams.Update(JoinRequest);
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Unable to respond to alliance join request. The stream is not JoinRequestStreamEntry!");
                            }
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to respond to alliance join request. The stream is null!");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to respond to alliance join request. The executer have an member role and is not permited to accept join request!");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to respond to alliance join request. Alliance member is null and this a major bug, Please inform server developer ASAP!");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to respond to alliance join request. Alliance is null and this a major bug, Please inform server developer ASAP!");
            }
        }
    }
}