namespace CR.Servers.CoC.Packets.Messages.Client.Alliances
{
    using System.Collections.Generic;
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.Extensions.Binary;

    internal class LeaveAllianceMessage : Message
    {
        public LeaveAllianceMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override short Type
        {
            get
            {
                return 14308;
            }
        }

        internal override void Process()
        {
            Level Level = this.Device.GameMode.Level;

            if (Level.Player.InAlliance)
            {
                Alliance Alliance = Level.Player.Alliance;
                Member Member;
                if (Alliance.Members.Quit(Level.Player.UserId, out Member))
                {
                    if (Member.Role == Role.Leader)
                    {
                        if (Alliance.Members.Slots.Count > 0)
                        {
                            Member NewLeader = null;

                            List<Member> Members = Alliance.Members.Slots.Values.ToList();
                            List<Member> CoLeaders = Members.FindAll(T => T.Role == Role.CoLeader);

                            if (CoLeaders.Count > 0)
                            {
                                NewLeader = CoLeaders.Count > 1 ? CoLeaders[Resources.Random.Next(0, CoLeaders.Count)] : CoLeaders[0];
                            }
                            else
                            {
                                List<Member> Elders = Members.FindAll(T => T.Role == Role.Elder);

                                if (Elders.Count > 0)
                                {
                                    NewLeader = Elders.Count > 1 ? Elders[Resources.Random.Next(0, Elders.Count)] : Elders[0];
                                }
                                else
                                {
                                    if (Members.Count > 0)
                                    {
                                        if (Members.Count > 1)
                                        {
                                            NewLeader = Members[Resources.Random.Next(0, Members.Count)];

                                            while (NewLeader == Member)
                                            {
                                                Logging.Error(this.GetType(), "Warning, this should never happen! Member should be previusly removed in Quit() function however Newleader is equal to the member!");
                                                NewLeader = Members[Resources.Random.Next(0, Members.Count)];
                                            }
                                        }
                                        else
                                        {
                                            NewLeader = Members[0];
                                        }
                                    }
                                    else
                                    {
                                        Logging.Error(this.GetType(), "Warning, this should never happen! Member is a chief and is going to leave the clan but the clan is now empty and the if statement for checking clan member count has not been triggered!");
                                    }
                                }
                            }

                            if (NewLeader != null)
                            {
                                //Logging.Info(this.GetType(), "The new leader name is " + NewLeader.Player.Name + ".");


                                if (NewLeader.Role == Role.Member)
                                {
                                    Alliance.Streams.AddEntry(new EventStreamEntry(NewLeader, Member, AllianceEvent.Promoted) {SenderRole = Role.Elder});
                                    Alliance.Streams.AddEntry(new EventStreamEntry(NewLeader, Member, AllianceEvent.Promoted) {SenderRole = Role.CoLeader});
                                }
                                else if (NewLeader.Role == Role.Elder)
                                {
                                    Alliance.Streams.AddEntry(new EventStreamEntry(NewLeader, Member, AllianceEvent.Promoted) {SenderRole = Role.CoLeader});
                                }
                                else if (NewLeader.Role == Role.Leader)
                                {
                                    Logging.Error(this.GetType(), "Error when leaving the clan, NewChief was a leader.");
                                }

                                NewLeader.Role = Role.Leader;
                                Alliance.Streams.AddEntry(new EventStreamEntry(NewLeader, Member, AllianceEvent.Promoted));

                                if (NewLeader.Player.Connected)
                                {
                                    NewLeader.Player.Level.GameMode.CommandManager.AddCommand(
                                        new Changed_Alliance_Role(this.Device)
                                        {
                                            AllianceId = Alliance.AllianceId,
                                            AllianceRole = Role.Leader
                                        });
                                }

                                Member.Role = Role.CoLeader;
                                Alliance.Streams.AddEntry(new EventStreamEntry(Member, Member, AllianceEvent.Demoted));
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Error when leaving the clan. New leader is null");
                            }
                        }
                    }

                    Alliance.Streams.AddEntry(new EventStreamEntry(Member, Member, AllianceEvent.Left));
                    this.Device.GameMode.CommandManager.AddCommand(new Leaved_Alliance(this.Device) {AllianceId = Level.Player.AllianceId});
                }
                else
                {
                    Logging.Error(this.GetType(), "Error when leaving the clan. Quit() returned false");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Error when leaving the clan. InAlliance returned false");
            }
        }
    }
}