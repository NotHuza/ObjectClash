namespace CR.Servers.CoC.Files
{
    using System;
    using System.IO;
    using CR.Servers.CoC.Core;
    using Newtonsoft.Json.Linq;

    public class Fingerprint
    {
        public static string Json;
        public static string Sha;
        public static string[] Version;

        public static bool Custom;

        public static void Initialize()
        {
            try
            {
                if (!Fingerprint.Patches())
                {
                    if (File.Exists(@"Gamefiles\fingerprint.json"))
                    {
                        Fingerprint.Json = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Gamefiles\fingerprint.json");
                        JObject _Json = JObject.Parse(Fingerprint.Json);
                        Fingerprint.Sha = _Json["sha"].ToObject<string>();
                        Fingerprint.Version = _Json["version"].ToObject<string>().Split('.');

                        Logging.Info(typeof(Fingerprint), "The Fingerprint has been loaded, with version " + string.Join(".", Fingerprint.Version) + ".");
                    }
                    else
                    {
                        Logging.Info(typeof(Fingerprint), "The Fingerprint cannot be loaded, the file does not exist.");
                    }
                }
                else
                {
                    Logging.Info(typeof(Fingerprint), "The Fingerprint is loaded and custom, with version " + string.Join(".", Fingerprint.Version) + ".");
                }
            }
            catch (Exception Exception)
            {
                Logging.Info(typeof(Fingerprint), Exception.GetType().Name + " while parsing the fingerprint.");
            }
        }

        private static bool Patches()
        {
            bool _Result = false;

            if (File.Exists(Directory.GetCurrentDirectory() + "\\Patchs\\VERSION"))
            {
                string[] _Lines = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\Patchs\\VERSION");

                if (!string.IsNullOrEmpty(_Lines[0]))
                {
                    Fingerprint.Version = _Lines[0].Split('.');

                    if (_Lines.Length > 1 && !string.IsNullOrEmpty(_Lines[1]))
                    {
                        Fingerprint.Sha = _Lines[1];

                        if (File.Exists(Directory.GetCurrentDirectory() + "\\Patchs\\" + Fingerprint.Sha + "\\fingerprint.json"))
                        {
                            Fingerprint.Json = File.ReadAllText(Directory.GetCurrentDirectory() + "\\Patchs\\" + Fingerprint.Sha + "\\fingerprint.json");
                            Fingerprint.Json.Trim('\n', '\r');

                            _Result = true;
                            Fingerprint.Custom = true;
                        }
                    }
                }
            }

            return _Result;
        }
    }
}