using CR.Servers.CoC.Core;
using CR.Servers.Core.Consoles;
using Microsoft.Extensions.Configuration;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;

namespace CR.Servers.CoC
{
    internal class Program
    {
        private static int Width = 140;
        private static int Height = 30;

        public static IConfigurationRoot Configuration { get; set; }




        private static void Main()
        {
            Configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Config.json").Build();
            Console.Title =  $"Huza Clash - {Assembly.GetExecutingAssembly().GetName().Name} - {DateTime.Now.Year} ©";

            Console.SetOut(new Prefixed());
            Console.SetWindowSize(Width, Height);

            Servers.Core.Consoles.Colorful.Console.WriteWithGradient(@"
              ___ ___                        _________ .__                .__     
             /   |   \ __ _______________    \_   ___ \|  | _____    _____|  |__  
            /    ~    \  |  \___   /\__  \   /    \  \/|  | \__  \  /  ___/  |  \ 
            \    Y    /  |  //    /  / __ \_ \     \___|  |__/ __ \_\___ \|   Y  \
             \___|_  /|____//_____ \(____  /  \______  /____(____  /____  >___|  /
                   \/             \/     \/          \/          \/     \/     \/                                                                                                            Clash Edition
            ", Color.OrangeRed, Color.LimeGreen, 14);

            Console.ForegroundColor = ComsoleColor.Blue;
            Console.WriteLine(@"Ultarpowe Developer Team: Antz, Huza Modz, MeteHun, Alin");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(@"Huza Clash's programs are protected by our policies, available only to our partner.");
            Console.WriteLine(@"Huza Clash Republic's programs are under the 'Proprietary' license.");
            Console.WriteLine(@"Huza Clash is NOT affiliated to 'Supercell Oy'.");
            Console.WriteLine(@"Huza Clash does NOT own 'Clash of Clans', 'Boom Beach', 'Clash Royale'.");
            Console.WriteLine();
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " is now starting..." +  Environment.NewLine);
            
            Resources.Initialize();
            Thread.Sleep(-1);
        }

        public static void UpdateTitle()
        {
            Console.Title = Constants.Title + Resources.Devices.Count;
        }
    }
}
