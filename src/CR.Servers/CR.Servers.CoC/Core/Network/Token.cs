namespace CR.Servers.CoC.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    using CR.Servers.CoC.Logic;

    internal class Token : IDisposable
    {
        internal bool Aborting;
        internal SocketAsyncEventArgs Args;
        internal Device Device;
        internal List<byte> Packet;
        internal Socket Socket;

        internal Token(SocketAsyncEventArgs Args, Device Device, Socket socket)
        {
            this.Socket = socket;

            this.Device = Device;
            this.Device.Token = this;

            this.Args = Args;
            this.Args.UserToken = this;

            this.Packet = new List<byte>(Constants.ReceiveBuffer);
        }

        internal bool Connected
        {
            get
            {
                return !this.Aborting && this.Socket.Connected;
            }
        }

        public void Dispose()
        {
            if (this.Aborting)
            {
                return;
            }

            this.Aborting = true;

            this.Packet.Clear();
            this.Device.Dispose();

            if (this.Socket.Connected)
            {
                this.Socket.Close();
            }

            this.Packet = null;
            this.Device = null;
        }

        internal void SetData()
        {
            if (!this.Aborting)
            {
                byte[] Data = new byte[this.Args.BytesTransferred];
                Array.Copy(this.Args.Buffer, Data, this.Args.BytesTransferred);
                this.Packet.AddRange(Data);
            }
        }

        internal void Process()
        {
            if (!this.Aborting)
            {
                this.Device.Process(this.Packet.ToArray());
            }
        }
    }
}