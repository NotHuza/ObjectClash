using System;
using System.Drawing;
using System.Reflection;

namespace CRepublic.Restarter
{
    public static class ConsoleUtils
    {
        public static void Welcome()
        {
            CR.Servers.Core.Consoles.Colorful.Console.WriteWithGradient(@"
                     █████╗ ██████╗      ██╗███████╗ █████╗ ████████╗ ██████╗  ████████╗ ██████╗████████╗████████╗   
                    ██╔══██╗██╔══██╗     ██║██╔════╝██╔══██╗╚══██╔══╝██╔═══██╗ ██ ╔════╝██╔════╝██ ╔════╝╚══██╔══╝
                    ██║  ██║██████╦╝     ██║█████╗  ██║  ╚═╝   ██║   ██║████╔╝ █████╗   ╚█████╗ █████╗      ██║ 
                    ██║  ██║██╔══██╗██╗  ██║██╔══╝  ██║  ██╗   ██║   ██║   ██╗ ██╔══╝    ╚═══██╗██╔══╝      ██║
                    ╚█████╔╝██████╦╝╚█████╔╝███████╗╚█████╔╝   ██║   ██║    ██║████████╗██████╔╝████████╗   ██║
                     ╚════╝ ╚═════╝  ╚════╝ ╚══════╝ ╚════╝    ╚═╝   ╚═╝    ╚═╝ ╚══════╝╚═════╝  ╚══════╝   ╚═╝
            ", Color.OrangeRed, Color.LimeGreen, 14);              
            Console.WriteLine();
            WriteLineCenterDarkYellow(" RESTARTER V" + Assembly.GetExecutingAssembly().GetName().Version);


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