using System.Collections.Generic;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Files;
using CR.Servers.CoC.Files.CSV_Logic.Logic;
using CR.Servers.Core.Consoles.Colorful;
using CR.Servers.Extensions.List;
using Newtonsoft.Json.Linq;

namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Change_Battle_Troop : Command
    {
        public Change_Battle_Troop(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 711;
            }
        }

        internal int OldTroop;
        internal int NewTroop;

        internal override void Decode()
        {
            this.OldTroop = this.Reader.ReadInt32(); //Old Troop
            this.NewTroop = this.Reader.ReadInt32(); //New Troop
            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddInt(this.OldTroop);
            Data.AddInt(this.NewTroop);
            base.Encode(Data);
        }

        internal override void Execute()
        {
            /*var Battle = this.Device.Account.DuelBattle.GetBattle(this.Device.GameMode.Level);
            var Units = Battle.Recorder.Attacker["units2"] as JArray;

            int index = -1;

            for (int i = 0; i < Units.Count; i++)
            {
                JObject array = Units[i] as JObject;
                if ((int)array["id"] == this.OldTroop)
                {
                    index = i;
                    break;
                }
            }

            if (index > -1)
            {
                var OldUnitData = CSV.Tables.GetWithGlobalId(this.OldTroop) as CharacterData;
                var NewUnitData = CSV.Tables.GetWithGlobalId(this.NewTroop) as CharacterData;
                var OldUnitLevel = Battle.Attacker.Player.GetUnitUpgradeLevel(OldUnitData);
                var NewUnitLevel = Battle.Attacker.Player.GetUnitUpgradeLevel(NewUnitData);

                Units[index]["cnt"] = (int)Units[index]["cnt"] - OldUnitData.UnitsInCamp[OldUnitLevel];
                Units.Add(new JObject
                {
                    {"id", NewUnitData.GlobalId},
                    {"cnt", NewUnitData.UnitsInCamp[NewUnitLevel] },
                });

                //Battle.SendLiveReplayInfo();
            }

            ShowValues();*/
            
        }
    }
}