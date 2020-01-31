using System;
using System.Reflection;

namespace CRepublic.Restarter
{
    public static class ConsoleUtils
    {
        public static void Welcome()
        {
            Console.WriteLine();
            WriteLineCenterYellow(@"_________ .__                .__                          __________                   ___.   .__  .__        ");
            WriteLineCenterYellow(@"\_   ___ \|  | _____    _____|  |__   ___________  ______ \______   \ ____ ______  __ _\_ |__ |  | |__| ____  ");
            WriteLineCenterYellow(@"/    \  \/|  | \__  \  /  ___/  |  \_/ __ \_  __ \/  ___/  |       _// __ \\____ \|  |  \ __ \|  | |  |/ ___\ ");
            WriteLineCenterYellow(@"\     \___|  |__/ __ \_\___ \|   Y  \  ___/|  | \/\___ \   |    |   \  ___/|  |_> >  |  / \_\ \  |_|  \  \___ ");
            WriteLineCenterYellow(@" \______  /____(____  /____  >___|  /\___  >__|  /____  >  |____|_  /\___  >   __/|____/|___  /____/__|\___  >");
            WriteLineCenterYellow(@"        \/          \/     \/     \/     \/           \/          \/     \/|__|             \/             \/ ");
            WriteLineCenterYellow(@"                                                                               RESTARTER V" + Assembly.GetExecutingAssembly().GetName().Version);


            WriteLineCenterGreen("Server Restarter has been loaded successfully.\n");
            WriteLineCenterYellow("Make sure to edit 'restarter.config' file for your needs!");
        }

        public static void WriteLineCenter(string value)
        {
            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
        }

        public static void WriteLineCenterDarkYellow(string value)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.ResetColor();
        }

        public static void WriteLineCenterGreen(string value)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.ResetColor();
        }

        public static void WriteLineCenterRed(string value)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.ResetColor();
        }

        public static void WriteLineCenterCyan(string value)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.ResetColor();
        }

        public static void WriteLineCenterYellow(string value)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
            Console.WriteLine(value);
            Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop);
            Console.ResetColor();
        }
    }
}
