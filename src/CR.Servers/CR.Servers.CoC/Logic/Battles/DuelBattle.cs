namespace CR.Servers.CoC.Logic.Battles
{
    using System.Collections.Generic;

    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic.Duel.Entry;
    using CR.Servers.CoC.Packets;
    using CR.Servers.CoC.Packets.Messages.Server.Avatar;
    using Core;

    internal class DuelBattle
    {
        internal int BattleId;

        internal bool Ended;

        internal Battle Battle1;
        internal Battle Battle2;

        internal DuelBattle(Battle battle1, Battle battle2)
        {
            this.Battle1 = battle1;
            this.Battle2 = battle2;
        }

        internal Battle GetBattle(Level level)
        {
            if (level.Player.UserId == this.Battle1.Attacker.Player.UserId)
            {
                return this.Battle1;
            }

            return level.Player.UserId == this.Battle2.Attacker.Player.UserId ? this.Battle2 : null;
        }

        internal Battle GetEnemyBattle(Level level)
        {
            if (level.Player.UserId == this.Battle1.Attacker.Player.UserId)
            {
                return this.Battle2;
            }

            return level.Player.UserId == this.Battle2.Attacker.Player.UserId ? this.Battle1 : null;
        }

        internal void HandleCommands(int subTick, List<Command> commands, Device device)
        {
            Battle battle = this.GetBattle(device.GameMode.Level);

            if (battle != null)
            {
                battle.HandleCommands(subTick, commands);
            }

            this.SendDuelBattleInfoMessage();
        }

        internal void SendDuelBattleInfoMessage()
        {
            if (!this.Ended)
            {
                if (this.Battle1.Ended)
                {
                    if (this.Battle2.Ended)
                    {
                        new Village2AttackEntryUpdateMessage(this.Battle1.Device, new Village2AttackProgressEntry(this.BattleId, this.Battle2, this.Battle1)).Send();
                        new Village2AttackEntryUpdateMessage(this.Battle2.Device, new Village2AttackProgressEntry(this.BattleId, this.Battle1, this.Battle2)).Send();

                        this.Ended = true;

                        DuelBattle _;
                        Resources.Duels.DuelBattles.TryRemove(BattleId, out _);

                        return;
                    }
                }

                new Village2AttackEntryAddedMessage(this.Battle1.Device, new Village2AttackProgressEntry(this.BattleId, this.Battle2, this.Battle1)).Send();
                new Village2AttackEntryAddedMessage(this.Battle2.Device, new Village2AttackProgressEntry(this.BattleId, this.Battle1, this.Battle2)).Send();
            }
        }
    }
}