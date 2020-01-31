//#define Console

namespace CR.Servers.CoC.Core
{
    using System;
    using System.Diagnostics;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions;

    internal static class Logging
    {
        internal static void Error(Type Type, string Message)
        {
            Resources.Logger.Error(Type.Name + " : " + Message);
#if Console
            Console.WriteLine("[*] " + ConsolePad.Padding(Type.Name, 15) + " : " + Message);
#else
            //Debug.WriteLine("[*] " + ConsolePad.Padding(Type.Name, 15) + " : " + Message);
#endif
        }

        [Conditional("DEBUG")]
        internal static void Info(Type Type, string Message)
        {
            Resources.Logger.Info(Type.Name + " : " + Message);
#if Console
            Console.WriteLine("[*] " + ConsolePad.Padding(Type.Name, 15) + " : " + Message);
#else
            //Debug.WriteLine("[*] " + ConsolePad.Padding(Type.Name, 15) + " : " + Message);
#endif
        }

        internal static void Error(Type Type, Device Device, string Message, bool ServerError = true)
        {
            Logging.Error(Type, Message);

            if (ServerError)
            {
                //new Server_Error_Message(Device, Message).Send();
            }
        }
    }
}