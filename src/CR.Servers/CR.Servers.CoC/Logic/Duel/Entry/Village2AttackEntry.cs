namespace CR.Servers.CoC.Logic.Duel.Entry
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic.Battles;
    using CR.Servers.Extensions.List;

    internal class Village2AttackEntry
    {
        internal int RemainingTime;
        internal long BattleId;
        internal Battle Battle;

        internal Village2AttackEntry(long battleId, Battle battle)
        {
            this.BattleId = battleId;
            this.Battle = battle;

            this.RemainingTime = -1;
        }

        internal virtual int GetEntryType()
        {
            return 0;
        }

        internal virtual void Encode(List<byte> packet)
        {
            packet.AddBools(false, false);
            {
                packet.AddLong(this.BattleId);

                if (this.Battle.Attacker.Player.Alliance != null)
                {
                    packet.AddBool(true);
                    packet.AddLong(this.Battle.Attacker.Player.AllianceId);
                    packet.AddString(this.Battle.Attacker.Player.Alliance.Header.Name);
                    packet.AddInt(this.Battle.Attacker.Player.Alliance.Header.Badge);
                    packet.AddInt(this.Battle.Attacker.Player.Alliance.Header.ExpLevel);
                }
                else
                {
                    packet.AddBool(false);
                }

                packet.AddLong(this.Battle.Attacker.Player.UserId);
                packet.AddLong(this.Battle.Attacker.Player.UserId);
                packet.AddString(this.Battle.Attacker.Player.Name);
                packet.AddInt(2);

                packet.AddBool(false);
                {
                    // packet.AddLong(1);
                }

                packet.AddVInt(15000);
                packet.AddVInt(15000);
                packet.AddVInt(15000);
                packet.AddVInt(15000);
                packet.AddBool(false);
                packet.AddVInt(this.Battle.RemainingBattleTimeSecs);
            }
        }
    }
}