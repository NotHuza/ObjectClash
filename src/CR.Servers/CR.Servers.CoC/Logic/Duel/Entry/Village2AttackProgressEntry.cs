namespace CR.Servers.CoC.Logic.Duel.Entry
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic.Battles;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;

    internal class Village2AttackProgressEntry : Village2AttackEntry
    {
        internal Battle Battle2;

        internal int State;

        internal Village2AttackProgressEntry(long battleId, Battle battle, Battle battle2) : base(battleId, battle)
        {
            this.Battle2 = battle2;
            this.State = battle.Ended && battle2.Ended ? 2 : 0;
        }

        internal override int GetEntryType()
        {
            return 1;
        }

        internal override void Encode(List<byte> packet)
        {
            base.Encode(packet);

            packet.AddVInt(this.State); // State: 0: Loss     1: Win     2: Draw

            packet.AddInt(3); // Stars
            packet.AddInt(100);
            packet.AddInt(3); // Enemy Stars
            packet.AddInt(100);
            packet.AddInt(0); // Gold2 Gains
            packet.AddInt(0); // Elixir2 Gains
            packet.AddInt(0); // Score Gains

            if (this.Battle.Replay != null)
            {
                packet.AddBool(true);
                packet.AddInt(this.Battle.Replay.HighId);
                packet.AddInt(this.Battle.Replay.LowId);
                packet.AddInt(0); // ReplayShardId
                packet.AddInt(9); // Major
                packet.AddInt(256); // Minor
                packet.AddInt(0); // ContentVersion
            }
            else
            {
                packet.AddBool(false);
            }

            if (this.Battle2.Replay != null)
            {
                packet.AddBool(true);
                packet.AddInt(this.Battle2.Replay.HighId);
                packet.AddInt(this.Battle2.Replay.LowId);
                packet.AddInt(0); // ReplayShardId
                packet.AddInt(9); // Major
                packet.AddInt(256); // Minor
                packet.AddInt(0); // ContentVersion
            }
            else
            {
                packet.AddBool(false);
            }

            packet.AddInt(31);
            packet.AddString(this.Battle.BattleLog.Save().ToString(Formatting.None));
            packet.AddString(this.Battle2.BattleLog.Save().ToString(Formatting.None));
        }
    }
}