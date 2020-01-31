using CR.Servers.CoC.Core.Consoles;
using CR.Servers.CoC.Core.Database;
using CR.Servers.CoC.Core.Events;
using CR.Servers.CoC.Core.Network;
using CR.Servers.CoC.Extensions.Game;
using CR.Servers.CoC.Files;
using CR.Servers.CoC.Logic;
using CR.Servers.CoC.Logic.Chat;
using CR.Servers.CoC.Logic.Slots;
using CR.Servers.CoC.Packets;
using CR.Servers.Core;
using CR.Servers.Logic.Enums;
using NLog;
using System;
using System.Text.RegularExpressions;

namespace CR.Servers.CoC.Core
{
    internal class Resources
    {
        internal static Devices Devices;
        internal static Accounts Accounts;
        internal static Battles Battles;
        internal static Clans Clans;
        internal static Chats Chats;
        internal static XorShift Random;
        internal static Gateway Gateway;
        internal static Regex Regex;
        internal static Regex Name;
        internal static Timers Timers;
        internal static Test Test;
        internal static Logger Logger;
        internal static Duels Duels;
        internal static Processor Processor;

        internal static bool Started;
        internal static bool Closing;

        internal static void Initialize()
        {
            Resources.Logger = LogManager.GetCurrentClassLogger(typeof(Resources));

            Factory.Initialize();
            CSV.Initialize();
            LevelFile.Initialize();
            GameEvents.Initialize();
            Globals.Initialize();
            Settings.Initialize();
            Fingerprint.Initialize();

            if (Constants.Database == DBMS.Mongo)
            {
                Mongo.Initialize();
            }

            Resources.Regex = new Regex("[ ]{2,}", RegexOptions.Compiled);
            Resources.Name = new Regex("^[a-zA-Z0-9- ]*$");

            Resources.Processor = new Processor();

            Resources.Devices = new Devices();
            Resources.Accounts = new Accounts();
            Resources.Battles = new Battles();
            Resources.Clans = new Clans();
            Resources.Chats = new Chats();
            Resources.Duels = new Duels();
            Resources.Random = new XorShift();
            Resources.Gateway = new Gateway();
            Resources.Timers = new Timers();
            Resources.Started = true;

            Parser.Initialize();
            EventsHandler.Initialize();

#if DEBUG
            Console.WriteLine("We loaded " + Factory.Messages.Count + " messages, " + Factory.Commands.Count + " commands, and " + Factory.Debugs.Count + " debug commands.\n");
            Resources.Test = new Test();
#endif
        }
    }
}