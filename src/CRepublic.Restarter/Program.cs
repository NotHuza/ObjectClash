using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace CRepublic.Restarter
{
    public class Program
    {
        public static Restarter Restarter { get; set; }
        public static string Version { get; set; }

        public static void Main(string[] args)
        {
            const int GWL_EXSTYLE = -20;
            const int WS_EX_LAYERED = 0x80000;
            const uint LWA_ALPHA = 0x2;
            //int LWA_COLORKEY = 0x1;

            // Obtain our handle (hWnd)
            var Handle = GetConsoleWindow();
            SetWindowLong(Handle, GWL_EXSTYLE, GetWindowLong(Handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
            // Opacity = 0.5 = (255/2)
            SetLayeredWindowAttributes(Handle, 0, 210, LWA_ALPHA);

            if (Console.WindowHeight != 35 || Console.WindowWidth != 130)
                Console.SetWindowSize(130, 35);

            Console.Title = "Clashers' Republic Restarter v" + Assembly.GetExecutingAssembly().GetName().Version +
                            " - Not Running";
            Console.Clear();
            ConsoleUtils.Welcome();

            // Make sure we have at least 1 argument.
            if (args.Length < 1)
            {
                ConsoleUtils.WriteLineCenterGreen("||");
                ConsoleUtils.WriteLineCenterGreen("Loaded file path from config: " +
                                                  ConfigurationManager.AppSettings["FileName"]);

                var filePath = ConfigurationManager.AppSettings["FileName"];
                args = new[]
                {
                    filePath
                };
            }

            // Make sure the argument (file) provided exists.
            if (!File.Exists(args[0]))
            {
                ConsoleUtils.WriteLineCenterRed("||");
                ConsoleUtils.WriteLineCenterRed(
                    $"File '{args[0]}' does not exists! Check your 'restarter.config' file and try again.");
                ConsoleUtils.WriteLineCenterYellow("Press ENTER to exit...");
                while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;

                Environment.Exit(0);
            }

            // Make sure the argument (file) provided is an .exe file.
            if (Path.GetExtension(args[0]) != ".exe")
            {
                ConsoleUtils.WriteLineCenterRed("||");
                ConsoleUtils.WriteLineCenterRed(
                    $"File '{args[0]}' is not a .exe! Check your 'restarter.config' file and try again.");
                ConsoleUtils.WriteLineCenterYellow("Press ENTER to exit...");

                while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;

                Environment.Exit(0);
            }

            // Make sure the argument (file) provided is not the restarter itself.
            var restarterEXE = Assembly.GetExecutingAssembly().ManifestModule.ToString();

            if (Path.GetFileName(args[0]) == restarterEXE)
            {
                ConsoleUtils.WriteLineCenterRed("||");
                ConsoleUtils.WriteLineCenterRed(
                    $"File '{args[0]}' is the restarter itself! Check your 'restarter.config' file and try again.");
                ConsoleUtils.WriteLineCenterYellow("Press ENTER to exit...");

                while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;

                Environment.Exit(0);
            }

            Console.Title = "Clashers' Republic Restarter";

            // Pass argument to the Restarter.
            Restarter = new Restarter(args[0]);
            Restarter.Start();

            Thread.Sleep(Timeout.Infinite);
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(int hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetLayeredWindowAttributes(int hWnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowLong(int hWnd, int nIndex);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int GetConsoleWindow();
    }
}
