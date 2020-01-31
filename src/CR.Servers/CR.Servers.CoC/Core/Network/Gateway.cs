using CR.Servers.CoC.Logic;
using CR.Servers.Logic.Enums;
using System;
using System.Net;
using System.Net.Sockets;

namespace CR.Servers.CoC.Core.Network
{
    internal class Gateway
    {
        private readonly Socket _listener;
        private readonly SocketAsyncEventArgsPool _sendPool;

        internal Gateway()
        {
            _sendPool = new SocketAsyncEventArgsPool();

            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, 9339));
            _listener.Listen(500);


            Logging.Info(this.GetType(), "Gateway started :)");

            SocketAsyncEventArgs acceptEvent = new SocketAsyncEventArgs();
            acceptEvent.Completed += this.OnAcceptCompleted;
            this.Accept(acceptEvent);
        }

        internal SocketAsyncEventArgs GetSendArgs()
        {
            var args = _sendPool.Dequeue();
            if (args == null)
                return new SocketAsyncEventArgs();

            return args;
        }

        internal void Accept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            if (!this._listener.AcceptAsync(args))
            {
                this.OnAcceptCompleted(null, args);
            }
        }

        internal void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                this.ProcessAccept(args);
            }

            this.Accept(args);
        }

        internal void ProcessAccept(SocketAsyncEventArgs args)
        {
            Socket socket = args.AcceptSocket;

            if (socket != null && socket.Connected)
            {
                SocketAsyncEventArgs readEvent = new SocketAsyncEventArgs();
                readEvent.SetBuffer(new byte[Constants.ReceiveBuffer], 0, Constants.ReceiveBuffer);
                readEvent.Completed += this.OnReceiveCompleted;
                readEvent.DisconnectReuseSocket = false;

                Device device = new Device();
                Token token = new Token(readEvent, device, socket);

                Resources.Devices.OnConnect(device);

                device.State = State.SESSION;

                try
                {
                    if (!socket.ReceiveAsync(readEvent))
                    {
                        this.OnReceiveCompleted(null, readEvent);
                    }
                }
                catch (Exception Exception)
                {
                    Logging.Error(this.GetType(), "Exception while starting to receive in ProcessAccept. trace: " + Exception);
                }
            }
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                this.ProcessReceive(args);
            }
            else
            {
                this.Disconnect(args);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs args)
        {
            try
            {
                if (args.BytesTransferred > 0)
                {
                    Token token = (Token)args.UserToken;

                    if (token != null && !token.Aborting)
                    {
                        if (token.Socket.Connected)
                        {
                            token.SetData();

                            try
                            {
                                if (token.Socket.Available == 0)
                                {
                                    token.Process();
                                }

                                if (!token.Aborting)
                                {
                                    try
                                    {
                                        if (!token.Socket.ReceiveAsync(args))
                                        {
                                            this.ProcessReceive(args);
                                        }
                                    }
                                    catch (Exception Exception)
                                    {
                                        Logging.Error(this.GetType(), "Exception while starting to receive in ProcessReceive. trace: " + Exception);
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                Logging.Error(this.GetType(), "An error has been throwed when the handle of data. trace: " + exception);
                                this.Disconnect(args);
                            }
                        }
                        else
                        {
                            this.Disconnect(args);
                        }
                    }
                    else
                    {
                        this.Disconnect(args);
                    }
                }
                else
                {
                    this.Disconnect(args);
                }
            }
            catch (Exception Exception)
            {
                Logging.Error(this.GetType(), "Exception in ProcessReceive. trace: " + Exception);
            }
        }

        internal void Send(byte[] packet, Token token)
        {
            if (!token.Aborting)
            {
                if (token.Socket.Connected)
                {
                    SocketAsyncEventArgs writeEvent = GetSendArgs();

                    writeEvent.Completed += this.OnSendCompleted;
                    writeEvent.UserToken = token;
                    writeEvent.SetBuffer(packet, 0, packet.Length);

                    if (!token.Socket.SendAsync(writeEvent))
                    {
                        this.OnSendCompleted(token.Socket, writeEvent);
                    }
                }
            }
        }

        private void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred == 0)
            {
                Disconnect(args);
                return;
            }

            var socket = (Socket)sender;
            if (args.BytesTransferred < args.Count)
            {
                var offset = args.Offset + args.BytesTransferred;
                var count = args.Count - offset;

                args.SetBuffer(offset, count);

                try
                {
                    if (!socket.SendAsync(args))
                        OnSendCompleted(socket, args);
                }
                catch (ObjectDisposedException)
                {
                    args.Dispose();
                }
                catch (Exception ex)
                {
                    Logging.Error(this.GetType(), "Exception in OnSendCompleted. trace: " + ex);
                }
            }
            else
            {
                args.Completed -= OnSendCompleted;
                _sendPool.Enqueue(args);
            }
        }

        internal void Disconnect(SocketAsyncEventArgs rcvArgs)
        {
            if (rcvArgs.UserToken != null)
            {
                Token token = (Token)rcvArgs.UserToken;

                if (token != null)
                {
                    Resources.Devices.OnDisconnect(token.Device);

                    if (!token.Aborting)
                    {
                        token.Dispose();
                    }

                    rcvArgs.UserToken = null;
                    rcvArgs.Dispose();
                }
            }
        }
    }
}