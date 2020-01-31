namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.CoC.Packets.Messages.Server.Account;

    internal class LogicDebug
    {
        [Conditional("DEBUG")]
        internal static void Execute(string Args, Player[] Players)
        {
            switch (Args)
            {
                case "Add 1000 Diamonds":
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Connected)
                        {
                            Player.Level.GameMode.CommandManager.AddCommand(new Diamonds_Added(Player.Level.GameMode.Device) {Count = 1000});
                        }
                    }

                    break;
                }

                case "Add 10000 Diamonds":
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Connected)
                        {
                            Player.Level.GameMode.CommandManager.AddCommand(new Diamonds_Added(Player.Level.GameMode.Device) {Count = 10000});
                        }
                    }

                    break;
                }

                case "Add 100000 Diamonds":
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Connected)
                        {
                            Player.Level.GameMode.CommandManager.AddCommand(new Diamonds_Added(Player.Level.GameMode.Device) {Count = 100000});
                        }
                    }

                    break;
                }
                case "Add 1000000 Diamonds":
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Connected)
                        {
                            Player.Level.GameMode.CommandManager.AddCommand(new Diamonds_Added(Player.Level.GameMode.Device) {Count = 100000});
                        }
                    }

                    break;
                }
                case "Fast Forward 1000":
                {
                    foreach (Player Player in Players)
                    {
                        Player.Level.FastForwardTime(1000);
                        if (Player.Connected)
                        {
                            new DisconnectedMessage(Player.Level.GameMode.Device).Send();
                        }
                    }

                    break;
                }

                case "Fast Forward 10000":
                {
                    foreach (Player Player in Players)
                    {
                        Player.Level.FastForwardTime(10000);
                        if (Player.Connected)
                        {
                            new DisconnectedMessage(Player.Level.GameMode.Device).Send();
                        }
                    }

                    break;
                }

                case "Fast Forward 100000":
                {
                    foreach (Player Player in Players)
                    {
                        Player.Level.FastForwardTime(100000);
                        if (Player.Connected)
                        {
                            new DisconnectedMessage(Player.Level.GameMode.Device).Send();
                        }
                    }

                    break;
                }
                case "Fast Forward 1000000":
                {
                    foreach (Player Player in Players)
                    {
                        Player.Level.FastForwardTime(100000);
                        if (Player.Connected)
                        {
                            new DisconnectedMessage(Player.Level.GameMode.Device).Send();
                        }
                    }

                    break;
                }
                /*case "Remove 1000 Diamonds":
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Connected)
                        {
                            Player.GameMode.CommandManager.AddCommand(new TransactionsRevokedCommand(1000));
                        }
                    }

                    break;
                }

                case "Remove 10000 Diamonds":
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Connected)
                        {
                            Player.GameMode.CommandManager.AddCommand(new TransactionsRevokedCommand(10000));
                        }
                    }

                    break;
                }

                case "Remove 100000 Diamonds":
                {
                    foreach (Player Player in Players)
                    {
                        if (Player.Connected)
                        {
                            Player.GameMode.CommandManager.AddCommand(new TransactionsRevokedCommand(100000));
                        }
                    }

                    break;
                }*/
            }
        }

        internal static string[] GetListOfCommands()
        {
            List<string> Commands = new List<string>
            {
                "Add 1000 Diamonds",
                "Add 10000 Diamonds",
                "Add 100000 Diamonds",
                "Add 1000000 Diamonds",
                "Fast Forward 1000",
                "Fast Forward 10000",
                "Fast Forward 100000",
                "Fast Forward 1000000"
            };


            return Commands.ToArray();
        }
    }
}