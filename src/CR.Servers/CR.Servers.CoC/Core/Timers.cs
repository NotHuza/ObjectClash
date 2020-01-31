namespace CR.Servers.CoC.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.CoC.Packets.Messages.Server.Account;
    using Math = System.Math;
    using Timer = System.Timers.Timer;

    internal class Timers : ConcurrentDictionary<ServerTimer, Timer>
    {
        internal bool Initialized;

        internal Timers()
        {
            if (this.Initialized)
            {
                return;
            }

            this.Save();
            this.Restart();
            this.Initialized = true;

            this.Run();
        }

        internal void Stop()
        {
            foreach (Timer Timer in this.Values)
            {
                Timer.Stop();
            }

            this.Initialized = false;
        }

        internal void Run()
        {
            if (this.Initialized)
            {
                foreach (Timer Timer in this.Values)
                {
                    Timer.Start();
                }
            }
        }

        internal void Save()
        {
            Timer Timer = new Timer
            {
                Interval = TimeSpan.FromHours(1).TotalMilliseconds,
                AutoReset = true
            };

            Timer.Elapsed += async (_Sender, _Args) =>
            {
                Resources.Accounts.Saves();
                await Task.WhenAll(Resources.Clans.Saves());

                Logging.Info(typeof(Timers), "Sucessfuly saved players and clans at " + DateTime.Now.ToString("T"));
            };

            if (!this.TryAdd(ServerTimer.DataSaving, Timer))
            {
                Logging.Error(this.GetType(), $"Failed to add DataSaving timer to ServerTimer dictionary");
            }
        }

        internal void KeepAlive()
        {
            Timer Timer = new Timer
            {
                Interval = 60000,
                AutoReset = true
            };

            Timer.Elapsed += (_Sender, _Args) => { };

            if (!this.TryAdd(ServerTimer.KeepAliveCheck, Timer))
            {
                Logging.Error(this.GetType(), $"Failed to add KeepAlive timer to ServerTimer dictionary");
            }
        }

        internal void Restart()
        {
            var interval = default(TimeSpan);
            if (int.TryParse(Extension.ParseConfigString("Server:RestartIntervalInMinute"), out int Minute))
            {
                interval = TimeSpan.FromMinutes(Minute);
            }
            else
            {
                interval = TimeSpan.FromMinutes(30);
            }

            if (Math.Abs(interval.TotalMinutes) > 1)
            {
                Console.WriteLine($"Server Restarter has been loaded successfully. Restart Interval : {interval.TotalSeconds} seconds");

                Timer Timer = new Timer
                {
                    Interval = interval.TotalMilliseconds,
                    AutoReset = true
                };

                Timer.Elapsed += async (_Sender, _Args) =>
                {
                    try
                    {
                        Resources.Closing = true;
                        await Task.WhenAll(Task.Run(() =>
                        {
                            var players = Resources.Accounts.GetAllPlayers();
                            foreach (Player Player in players)
                            {
                                if (Player.Connected)
                                {
                                    new DisconnectedMessage(Player.Level.GameMode.Device).Send();
                                }
                            }
                        }));

                        Resources.Accounts.Saves();
                        await Task.WhenAll(Resources.Clans.Saves());
                    }
                    catch (Exception Exception)
                    {
                        Logging.Error(Exception.GetType(), "[: Failed at " + DateTime.Now.ToString("T") + ']' + Environment.NewLine + Exception.StackTrace);
                        return;
                    }

                    Environment.Exit(0);
                };

                if (!this.TryAdd(ServerTimer.ServerRestart, Timer))
                {
                    Logging.Error(this.GetType(), $"Failed to add ServerRestart timer to ServerTimer dictionary");
                }
            }
            else
            {
                Console.WriteLine("Server Restarter has been disabled");
            }
        }
    }
}