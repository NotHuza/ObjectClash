using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CRepublic.Hasher
{
    internal static class Program
    {
        internal static string Hash(this string input)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        internal static string Hash(BufferedStream input)
        {
            var hash = new SHA1Managed().ComputeHash(input);
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

        internal static void Main(string[] args)
        {
            Console.SetOut(new Prefixed());
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@"_________ .__                .__                          __________                   ___.   .__  .__        ");
            Console.WriteLine(@"\_   ___ \|  | _____    _____|  |__   ___________  ______ \______   \ ____ ______  __ _\_ |__ |  | |__| ____  ");
            Console.WriteLine(@"/    \  \/|  | \__  \  /  ___/  |  \_/ __ \_  __ \/  ___/  |       _// __ \\____ \|  |  \ __ \|  | |  |/ ___\ ");
            Console.WriteLine(@"\     \___|  |__/ __ \_\___ \|   Y  \  ___/|  | \/\___ \   |    |   \  ___/|  |_> >  |  / \_\ \  |_|  \  \___ ");
            Console.WriteLine(@" \______  /____(____  /____  >___|  /\___  >__|  /____  >  |____|_  /\___  >   __/|____/|___  /____/__|\___  >");
            Console.WriteLine(@"        \/          \/     \/     \/     \/           \/          \/     \/|__|             \/             \/ ");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("Please enter your desired hash version");
            var Version = Console.ReadLine();

            var str = "{\"files\":[";

            var _files = Directory.GetFiles(args[0], args[3], SearchOption.AllDirectories);
            foreach (var path in _files)
            {
                var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(path));
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    using (var stream = new BufferedStream(fileStream))
                    {
                        str += "{\"sha\":\"" + Hash(stream) + "\",\"file\":\"" +
                               Path.Combine(directoryInfo.Name, Path.GetFileName(path)).Replace("\\", "\\/") + "\"},";
                        stream.Dispose();
                    }
                    fileStream.Dispose();
                }
            }

            var dateTimehash = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds
                .ToString(CultureInfo.InvariantCulture).Hash();
            var str1 = str.TrimEnd(',') + "],\"sha\":\"" + dateTimehash + "\",\"version\":\"" + Version + "\"}";
            Directory.CreateDirectory(args[1]);
            var destDirName = Path.Combine(args[1], dateTimehash);
            Directory.Move(args[0], destDirName);

            var textWriter =
                new StreamWriter(new FileStream(Path.Combine(destDirName, "fingerprint.json"), FileMode.CreateNew),
                    new UTF8Encoding(false));
            textWriter.Write(str1);
            textWriter.Dispose();

            if (File.Exists(Path.Combine(args[1], "VERSION")))
                File.Move(Path.Combine(args[1], "VERSION"), Path.Combine(args[1], "VERSION.Old"));
            var versionfile = new StringBuilder();
            versionfile.AppendLine(Version);
            versionfile.AppendLine(dateTimehash);


            var textWriter2 = new StreamWriter(new FileStream(Path.Combine(args[1], "VERSION"), FileMode.CreateNew),
                new UTF8Encoding(false));
            textWriter2.Write(versionfile.ToString());
            textWriter2.Dispose();
        }
    }
}