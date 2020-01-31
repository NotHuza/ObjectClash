using System;
using System.Diagnostics;
using CR.Servers.Extensions;

namespace CR.Servers.Core
{
    internal static class Logging
    {

        internal static void Error(Type Type, string Message)
        {
            Debug.WriteLine("[*] " + ConsolePad.Padding(Type.Name, 15) + " : " + Message);
        }

        [Conditional("DEBUG")]
        internal static void Info(Type Type, string Message)
        {
            Debug.WriteLine("[*] " + ConsolePad.Padding(Type.Name, 15) + " : " + Message);
        }
    }
}