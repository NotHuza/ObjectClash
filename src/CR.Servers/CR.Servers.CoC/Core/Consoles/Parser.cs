namespace CR.Servers.CoC.Core.Consoles
{
    using System;
    using System.Threading;
    using CR.Servers.CoC.Core.Events;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions;

    internal class Parser
    {
        internal static void Initialize()
        {
            new Thread(() =>
            {
                while (true)
                {
                    int CursorTop2 = Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
                    Console.Write("debug@HuzaClash.localnetwork, /help > ");

                    string Command = Console.ReadLine();

                    Console.SetCursorPosition(0, CursorTop2 - 1);
                    Console.WriteLine(new string(' ', Console.BufferWidth));
                    Console.SetCursorPosition(0, CursorTop2 - 2);

                    switch (Command)
                    {
                        case "/stats":
                            {
                                if (Resources.Started)
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("# " + DateTime.Now.ToString("d") + " ---- STATS ---- " + DateTime.Now.ToString("T") + " #");
                                    Console.WriteLine("# ----------------------------------- #");
                                    Console.WriteLine("# In-Memory Devices  # " + ConsolePad.Padding(Resources.Devices.Count.ToString(), 15) + "#");
                                    Console.WriteLine("# In-Memory Accounts # " + ConsolePad.Padding(Resources.Accounts.Count.ToString(), 15) + "#");
                                    Console.WriteLine("# In-Memory Clans    # " + ConsolePad.Padding(Resources.Clans.Count.ToString(), 15) + "#");
                                    Console.WriteLine("# In-Memory Duels    # " + ConsolePad.Padding(Resources.Duels.DuelBattles.Count.ToString(), 15) + "#");
                                    Console.WriteLine("# In-Memory Duels-Q  # " + ConsolePad.Padding(Resources.Duels.WaitingDeviceQueue.Count.ToString(), 15) + "#");
                                    Console.WriteLine("# ----------------------------------- #");

                                    Console.WriteLine("# Incoming-Processors ->" + "#".PadLeft(39 - "# Incoming-Processors ->".Length));
                                    foreach (var processor in Resources.Processor.IncomingThreads)
                                    {
                                        string line = $"# --- {processor.Count} In Queue";
                                        Console.WriteLine(line + "#".PadLeft(39 - line.Length));
                                    }

                                    Console.WriteLine("# Outgoing-Processors ->" + "#".PadLeft(39 - "# Outgoing-Processors ->".Length));
                                    foreach (var processor in Resources.Processor.OutgoingThreads)
                                    {
                                        string line = $"# --- {processor.Count} In Queue";
                                        Console.WriteLine(line + "#".PadLeft(39 - line.Length));
                                    }

                                    Console.WriteLine("# ----------------------------------- #");
                                }

                                break;
                            }

                        case "/test":
                            {
                                if (Resources.Started)
                                {
                                }

                                break;
                            }

                        case "/clear":
                            {
                                Console.Clear();
                                break;
                            }

                        case "/exit":
                        case "/shutdown":
                        case "/stop":
                            {
                                EventsHandler.Process();
                                Environment.Exit(-1);
                                break;
                            }
                        case "/help":
                            {
                                Console.WriteLine();
                                Console.WriteLine("!---- HELP ----!");
                                Console.WriteLine("# ------------------------------------- #");
                                Console.WriteLine("# ---------/stats  server status------- #");
                                Console.WriteLine("# --/stop,/exit,/shutdown stop server-- #");
                                Console.WriteLine("# --------/clear clear Screaen--------- #");
                                Console.WriteLine("# --/debug GetListOfCommands before use-#");
                                Console.WriteLine("# ------------------------------------- #");
                                break;
                            }

                        case "/debug":
                            {
                                string[] Names = LogicDebug.GetListOfCommands();

                                foreach (string name in Names)
                                {
                                    Console.WriteLine("[DEBUG] Logic : " + ConsolePad.Padding(name));
                                }

                                break;
                            }

                        default:
                            {
                                var players = Resources.Accounts.GetAllPlayers();
                                LogicDebug.Execute(Command, players);
                                Console.WriteLine();
                                break;
                            }
                    }
                }
            }).Start();
        }
    }
}