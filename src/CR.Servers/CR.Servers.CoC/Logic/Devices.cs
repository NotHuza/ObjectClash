using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using CR.Servers.CoC.Core;
using CR.Servers.Titan;

namespace CR.Servers.CoC.Logic
{
    public class Devices
    {
        private readonly ConcurrentDictionary<IntPtr, WeakReference<Device>> _connected;
        private static Thread _deadSocketCheck;

        public Devices()
        {
            _connected = new ConcurrentDictionary<IntPtr, WeakReference<Device>>();

            _deadSocketCheck = new Thread(DeadSocket);
            _deadSocketCheck.Start();
        }

        public int Count => _connected.Count;

        public void OnConnect(Device device)
        {
            if (_connected.TryAdd(device.Socket.Handle, new WeakReference<Device>(device)))
                Program.UpdateTitle();
        }

        public void OnDisconnect(Device device)
        {
            if (device == null || device.Socket == null)
                return;

            WeakReference<Device> _;
            if (_connected.TryRemove(device.Socket.Handle, out _))
                Program.UpdateTitle();
        }

        private void DeadSocket()
        {
            while (true)
            {
                KeyValuePair<IntPtr, WeakReference<Device>>[] Devices = this._connected.ToArray();
                LogicArrayList<Device> DeadSockets = new LogicArrayList<Device>();

                foreach (var kv in Devices)
                {
                    Device Device;
                    if (kv.Value.TryGetTarget(out Device))
                    {
                        if (!Device.Connected)
                        {
                            DeadSockets.Add(Device);
                        }
                    }
                    else
                    {
                        WeakReference<Device> _;
                        this._connected.TryRemove(kv.Key, out _);
                    }
                }

                for (int i = 0; i < DeadSockets.Count; i++)
                {
                    var Device = DeadSockets[i];
                    if (!Device.Disposed)
                    {
                        Resources.Gateway.Disconnect(Device.Token.Args);
                    }
                }

                Thread.Sleep(30000);
            }
        }
    }
}
