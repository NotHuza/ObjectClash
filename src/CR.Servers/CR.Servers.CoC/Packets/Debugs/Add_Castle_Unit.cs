using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Files;
using CR.Servers.CoC.Files.CSV_Logic.Logic;
using CR.Servers.CoC.Logic;
using CR.Servers.CoC.Logic.Enums;
using CR.Servers.CoC.Packets.Commands.Server;
using CR.Servers.CoC.Packets.Messages.Server.Avatar;
using CR.Servers.CoC.Packets.Messages.Server.Home;
using CR.Servers.Logic.Enums;
using NLog.Targets;

namespace CR.Servers.CoC.Packets.Debugs
{
    internal class Add_Castle_Unit : Debug
    {
        public Add_Castle_Unit(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Add_Spells
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Player;
            }
        }

        internal override void Process()
        {
            if (this.Parameters.Length >= 1)
            {
                int Id;
                if (int.TryParse(this.Parameters[0], out Id))
                {
                    CharacterData data = (CharacterData)CSV.Tables.Get(Gamefile.Characters).GetDataWithInstanceID(Id);

                    Level level = this.Device.GameMode.Level;
                    level.Player.AllianceUnits.Clear();

                    int CastleMax = level.Player.CastleTotalCapacity;

                    do
                    {
                        int UnitLevel = level.Player.GetUnitUpgradeLevel(data);
                        this.Device.GameMode.CommandManager.AddCommand(new Alliance_Unit_Received(this.Device)
                        {
                            Donator = "❤DebugCommand❤",
                            UnitType = 0,
                            UnitId = data.GlobalId,
                            Level = UnitLevel
                        });


                        level.Player.AllianceUnits.Add(data.GlobalId, 1, UnitLevel);
                        CastleMax -= data.HousingSpace;

                    } while (CastleMax >= data.HousingSpace);
                }
            }
            else
            {
                new AvatarStreamEntryMessage(this.Device)
                {
                    StreamEntry = new AllianceMailAvatarStreamEntry(this.Device.GameMode.Level.Player)
                    {
                        LowId = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                        SenderName = "[System] Command Manager",
                        SenderLeague = 22,
                        Message = Constants.DonationHelp.ToString()
                    }
                }.Send();

                this.SendChatMessage("Please check your MailBox!");
            }
        }
    }
}