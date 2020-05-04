namespace CR.Servers.CoC.Packets.Debugs
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Files;
    using CR.Servers.Logic.Enums;
    using System;
    using CR.Servers.CoC.Extensions;

    internal class Server_Status : Debug
    {
        public Server_Status(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Server_Status
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
            SendChatMessage("Server Status : \n" +
                "Build Version : 20.9 (for 9.256) \n" +
                "Fingerprint SHA : " + (Fingerprint.Sha + "\n") +
                "Online Players : " + (Resources.Devices.Count.ToString() + " \n" +
                "In-Memory Players :" + (Resources.Accounts.Count.ToString() + " \n" +
                "In-Memory Clans :" + (Resources.Clans.Count.ToString() + " \n" +
                "Server Uptime :" + (DateTime.UtcNow.Subtract(Resources.StartTime).ToReadableString() + " \n" +
                "RAM usage of the Emulator : " + System.Diagnostics.Process.GetCurrentProcess().WorkingSet64 / (1024 * 1024) + " \n")))));
        }
    }
}