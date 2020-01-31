namespace CR.Servers.CoC.Core.Events
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Messages.Server.Account;

    internal class EventsHandler
    {
        private static EventHandler EHandler;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler Handler, bool Enabled);

        internal static void Initialize()
        {
            EventsHandler.EHandler += EventsHandler.Process;
            EventsHandler.SetConsoleCtrlHandler(EventsHandler.EHandler, true);
        }

        internal static void Process()
        {
            Console.WriteLine("- SHUTDOWN -");

            Logging.Info(typeof(EventHandler), "Server is shutting down, disconnecting player...");
            Resources.Closing = true;
            Task.Run(() =>
            {
                var players = Resources.Accounts.GetAllPlayers();
                foreach (Player Player in players)
                {
                    if (Player.Connected)
                    {
                        new DisconnectedMessage(Player.Level.GameMode.Device).Send();
                    }
                }
            });


            Task.WaitAll(Resources.Accounts.SaveAll());
            Task.WaitAll(Resources.Clans.SaveAll());
            //Task AllSave = Task.Run(() => Resources.Clans.Save());

            //AllSave.Wait();
            //Message.Wait();
        }

        private delegate void EventHandler();
    }
}