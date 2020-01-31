namespace CR.Servers.CoC.Logic.Chat
{
    using System.Collections.Generic;
    using System.Linq;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Packets.Messages.Server.Home;
    using System.Collections.Concurrent;
    using System;

    internal class Chat
    {
        internal ConcurrentDictionary<IntPtr, Device> Devices;

        public Chat()
        {
            this.Devices = new ConcurrentDictionary<IntPtr, Device>();
        }

        internal bool TryAdd(Device Device)
        {
            if (this.Devices.Count < 50)
            {
                return this.Devices.TryAdd(Device.Token.Socket.Handle, Device);
            }

            return false;
        }

        internal void Quit(Device Device)
        {
            Device _;
            this.Devices.TryRemove(Device.Token.Socket.Handle, out _);
        }

        internal void AddEntry(Device Device, string Message)
        {
            /*
            Device[] Devices = this.Devices.ToArray();
            */

            if (Devices.ContainsKey(Device.Token.Socket.Handle))
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    foreach (Device Device2 in Devices.Values)
                    {
                        if (Device2.Connected)
                        {
                            new GlobalChatLineMessage(Device2, Device.GameMode.Level.Player) { Message = Message }.Send();
                        }
                        else
                        {
                            this.Quit(Device2);
                        }
                    }

                    /*if (Device.Connected)
                    {
                        new Global_Chat_Line(Device, Device.GameMode.Level.Player) { Message = Message, Name = "You"}.Send();
                    }
                    else
                        this.Quit(Device);*/
                }
            }
        }
    }
}