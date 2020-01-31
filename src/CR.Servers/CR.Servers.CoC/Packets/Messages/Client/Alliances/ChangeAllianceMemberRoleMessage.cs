namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.Extensions.Binary;

    internal class ChangeAllianceMemberRoleMessage : Message
    {
        internal int HighId;
        internal int LowId;
        internal Role Role;

        public ChangeAllianceMemberRoleMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14306;
            }
        }

        internal override void Decode()
        {
            this.HighId = this.Reader.ReadInt32();
            this.LowId = this.Reader.ReadInt32();
            this.Role = (Role) this.Reader.ReadInt32();
        }

        internal override void Process()
        {
            Level Level = this.Device.GameMode.Level;
            Player Target = Resources.Accounts.LoadAccount(this.HighId, this.LowId)?.Player;

            if (Level.Player.InAlliance)
            {
                if (Target != null)
                {
                    if (Target.InAlliance)
                    {
                        Alliance Alliance = Level.Player.Alliance;
                        Member Executer = Level.Player.AllianceMember;
                        Member TargetMember = Target.AllianceMember;

                        if (Executer != null)
                        {
                            if (TargetMember != null)
                            {
                                Role ExecuterRole = Executer.Role;
                                Role CurrentRole = TargetMember.Role;

                                if (ExecuterRole == Role.Leader || ExecuterRole == Role.CoLeader)
                                {
                                    if (this.Role == Role.Leader)
                                    {
                                        Executer.Role = Role.CoLeader;
                                        TargetMember.Role = Role.Leader;

                                        Alliance.Streams.AddEntry(new EventStreamEntry(TargetMember, Executer, AllianceEvent.Promoted));
                                        Alliance.Streams.AddEntry(new EventStreamEntry(Executer, Executer, AllianceEvent.Demoted));

                                        if (this.Device.Connected)
                                        {
                                            this.Device.GameMode.CommandManager.AddCommand(
                                                new Changed_Alliance_Role(this.Device)
                                                {
                                                    AllianceId = Level.Player.AllianceId,
                                                    AllianceRole = Role.CoLeader
                                                });
                                        }

                                        if (Target.Connected)
                                        {
                                            Target.Level.GameMode.CommandManager.AddCommand(
                                                new Changed_Alliance_Role(Target.Level.GameMode.Device)
                                                {
                                                    AllianceId = Level.Player.AllianceId,
                                                    AllianceRole = Role.Leader
                                                });
                                        }
                                    }
                                    else
                                    {
                                        TargetMember.Role = this.Role;
                                        if (Target.Connected)
                                        {
                                            Target.Level.GameMode.CommandManager.AddCommand(
                                                new Changed_Alliance_Role(Target.Level.GameMode.Device)
                                                {
                                                    AllianceId = Level.Player.AllianceId,
                                                    AllianceRole = this.Role
                                                });
                                        }

                                        Alliance.Streams.AddEntry(new EventStreamEntry(TargetMember, Executer, CurrentRole.Superior(this.Role) ? AllianceEvent.Promoted : AllianceEvent.Demoted));
                                    }
                                }
                                else
                                {
                                    Logging.Error(this.GetType(), "Unable to change alliance member role. The executer doesn't have proper rank!");
                                }
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Unable to change alliance member role. The targetmember is null and this is critical is VerifyAlliance() seems to be failing. Please notify server deverloper as soon as possible!");
                            }
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to change alliance member role. The executer is null and this is critical is VerifyAlliance() seems to be failing. Please notify server deverloper as soon as possible!");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to change alliance member role. The target is not in a clan!");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to change alliance member role. The target is null");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to change alliance member role. The player is not in a clan!");
            }
        }
    }
}