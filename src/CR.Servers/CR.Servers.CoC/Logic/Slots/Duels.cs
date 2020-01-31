using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Logic.Battles;
using CR.Servers.CoC.Packets.Messages.Server.Battle;
using CR.Servers.CoC.Packets.Messages.Server.Home;

namespace CR.Servers.CoC.Logic.Slots
{
    internal class Duels
    {
        internal int Seed;

        internal Thread Matchmaking;
        internal ConcurrentDictionary<long, DuelBattle> DuelBattles;
        internal ConcurrentDictionary<long, Device> WaitingDeviceQueue;

        internal Duels()
        {
            this.Matchmaking = new Thread(this.MatchmakingTask);
            this.DuelBattles = new ConcurrentDictionary<long, DuelBattle>();
            this.WaitingDeviceQueue = new ConcurrentDictionary<long, Device>();

            this.Matchmaking.Start();
        }

        internal void MatchmakingTask()
        {
            while (true)
            {
                List<long> deviceKeys = this.WaitingDeviceQueue.Keys.ToList();

                for (int i = 0; i + 1 < deviceKeys.Count; i++)
                {
                    Device attacker1;
                    if (this.WaitingDeviceQueue.TryRemove(deviceKeys[i], out attacker1))
                    {
                        retry:

                        Device attacker2;
                        if (this.WaitingDeviceQueue.TryRemove(deviceKeys[++i], out attacker2))
                        {
                            DuelBattle duelBattle = new DuelBattle(new Battle(attacker1, attacker1.GameMode.Level, attacker2.GameMode.Level, true), new Battle(attacker2, attacker2.GameMode.Level, attacker1.GameMode.Level, true));

                            attacker1.Account.DuelBattle = duelBattle;
                            attacker2.Account.DuelBattle = duelBattle;

                            duelBattle.BattleId = Interlocked.Increment(ref this.Seed);

                            if (this.DuelBattles.TryAdd(duelBattle.BattleId, duelBattle))
                            {
                                new Village2AttackAvatarDataMessage(attacker1)
                                {
                                    Enemy = attacker2.GameMode.Level
                                }.Send();

                                new Village2AttackAvatarDataMessage(attacker2)
                                {
                                    Enemy = attacker1.GameMode.Level
                                }.Send();
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Unable to start duel battle.");
                            }
                        }
                        else
                        {
                            if (i + 1 < deviceKeys.Count)
                            {
                                goto retry;
                            }

                            this.WaitingDeviceQueue.TryAdd(deviceKeys[i], attacker1);
                        }
                    }
                }

                Thread.Sleep(500);
            }
        }

        internal void Join(Device device)
        {
            if (device.Account != null)
            {
                this.WaitingDeviceQueue.TryAdd(device.Account.AccountId, device);
            }
        }

        internal void Quit(Device device)
        {
            if (device.Account != null)
            {
                Device _;
                if (this.WaitingDeviceQueue.TryRemove(device.Account.AccountId, out _))
                {
                    new OwnHomeDataMessage(device).Send();
                }
            }
        }
    }
}