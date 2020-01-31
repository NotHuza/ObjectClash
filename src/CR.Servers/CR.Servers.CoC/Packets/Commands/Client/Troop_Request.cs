namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.Binary;

    internal class Troop_Request : Command
    {
        internal bool HaveMessage;
        internal string Message;

        public Troop_Request(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 511;
            }
        }

        internal override void Decode()
        {
            base.Decode();

            this.HaveMessage = this.Reader.ReadBoolean();

            if (this.HaveMessage)
            {
                this.Message = this.Reader.ReadString();
            }
        }

        internal override void Execute()
        {
           /* Player Player = this.Device.GameMode.Level.Player;

            if (Player.InAlliance)
            {
                Building Bunker = this.Device.GameMode.Level.GameObjectManager.Bunker;
                if (Bunker != null)
                {
                    BunkerComponent BunkerComponent = Bunker.BunkerComponent;
                    if (BunkerComponent != null)
                    {
                        if (BunkerComponent.CanSendUnitRequest)
                        {
                            Alliance Alliance = Player.Alliance;

                            foreach (StreamEntry entry in Alliance.Streams.Slots.Values.Where(T => T.SenderHighId == Player.HighID && T.SenderLowId == Player.LowID && T.Type == AllianceStream.Donate).ToArray())
                            {
                                Alliance.Streams.Remove(entry);
                            }

                            Alliance.Streams.AddEntry(new DonateStreamEntry(Player.AllianceMember)
                            {
                                MaxTroop = Player.CastleTotalCapacity,
                                MaxSpell = Player.CastleTotalSpellCapacity,
                                UsedTroopSend = Player.CastleUsedCapacity,
                                UsedTroop = Player.CastleUsedCapacity,
                                UsedSpell = Player.CastleUsedSpellCapacity,
                                Units = Player.AllianceUnits.Copy() ?? new AllianceUnitSlots(),
                                HaveMessage = this.HaveMessage,
                                Message = this.Message
                            });

                            BunkerComponent.UnitRequestTimer = new Timer();
                            BunkerComponent.UnitRequestTimer.StartTimer(Player.LastTick, Globals.AllianceTroopRequestCooldown);
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to request troop. The player BunkerComponent.CanSendUnitRequest returned false!");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to request troop. The player BunkerComponent is null!");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to request troop. The player doesn't have a bunker!");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to request troop. The player is not in a clan!");
            }*/
        }
    }
}