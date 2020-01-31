using System;
using System.IO;
using System.Text;

namespace CRepublic.Hasher
{
    internal class Prefixed : TextWriter
    {
        internal readonly TextWriter Original;

        internal Prefixed()
        {
            Original = Console.Out;
        }

        public override Encoding Encoding => new UTF8Encoding();

        public override void Write(string Message)
        {
            Original.Write("[Hasher]    {0}", Message);
        }

        public override void WriteLine(string Message)
        {
            try
            {
                if (Message.Length <= Console.WindowWidth)
                {
                    Console.SetCursorPosition((Console.WindowWidth - Message.Length) / 2, Console.CursorTop);
                }
            }
            catch
            {
            }

            Original.WriteLine("{0}", Message);
        }

        public override void WriteLine()
        {
            Original.WriteLine();
        }
    }
}