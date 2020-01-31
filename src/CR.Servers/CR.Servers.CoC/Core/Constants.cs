namespace CR.Servers.CoC.Core
{
    using System;
    using System.Reflection;
    using System.Text;
    using CR.Servers.Logic.Enums;

    internal class Constants
    {
        internal const int ServerId = 0;
        internal const int ReceiveBuffer = 4096;
        internal const int MaxPlayers = 17000;
        internal const int MaxSends = 500;

        internal const DBMS Database = DBMS.Mongo;

        internal static StringBuilder AIBaseHelp;
        internal static StringBuilder DonationHelp;
        internal static string Title = $"Clashers Republic - {Assembly.GetExecutingAssembly().GetName().Name} - {DateTime.Now.Year} © | Active Connections >";
    }
}