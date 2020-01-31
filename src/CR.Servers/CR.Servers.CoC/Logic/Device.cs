using System;
using System.Diagnostics;
using System.Linq;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Core.Network;
using CR.Servers.CoC.Logic.Mode;
using CR.Servers.CoC.Packets;
using CR.Servers.CoC.Packets.Stream;
using CR.Servers.Extensions.Binary;
using CR.Servers.Logic.Enums;
using System.Collections.Concurrent;
using System.Net.Sockets;
using CR.Servers.CoC.Packets.Messages.Server.Home;
using System.Collections.Generic;

namespace CR.Servers.CoC.Logic
{
    public class Device : IDisposable
    {
        internal Account Account;
        internal Chat.Chat Chat;

        internal bool Disposed;

        internal int EncryptionSeed;

        internal GameMode GameMode;
        internal DateTime LastKeepAlive;
        internal DeviceInfo Info;

        internal StreamEncrypter ReceiveEncrypter;
        internal StreamEncrypter SendEncrypter;

        internal State State = State.DISCONNECTED;

        internal Token Token;
        internal bool UseRC4;

        internal readonly KeepAliveServerMessage KeepAliveServerMessage;

        private readonly ConcurrentQueue<Message> _outgoingMessages;
        private readonly ConcurrentQueue<Message> _incomingMessages;

        public Device()
        {
            this.GameMode = new GameMode(this);
            this.LastKeepAlive = DateTime.UtcNow;

            KeepAliveServerMessage = new KeepAliveServerMessage(this);
            _outgoingMessages = new ConcurrentQueue<Message>();
            _incomingMessages = new ConcurrentQueue<Message>();
        }

        public Socket Socket => Token.Socket;

        public int Checksum
        {
            get
            {
                int checksum = 0;

                checksum += this.GameMode.Time.SubTick;
                checksum += this.GameMode.Time.TotalSecs;

                if (this.GameMode.Level.Player != null)
                {
                    checksum += this.GameMode.Level.Player.Checksum;

                    if (this.GameMode.Level.GameObjectManager != null)
                        checksum += this.GameMode.Level.GameObjectManager.Checksum;
                }

                // Visitor

                checksum += checksum;

                return checksum;
            }
        }

        internal long TimeSinceLastKeepAlive => (long)DateTime.UtcNow.Subtract(this.LastKeepAlive).TotalSeconds;

        internal bool Connected => !this.Disposed && this.Token.Connected;

        internal string OS => this.Info.Android ? "Android" : "iOS";

        public void EnqueueOutgoingMessage(Message message)
        {
            _outgoingMessages.Enqueue(message);
        }

        public void EnqueueIncomingMessage(Message message)
        {
            _incomingMessages.Enqueue(message);
        }

        public void Flush()
        {
            if (_outgoingMessages.Count > 0)
            {
                var queueId = Resources.Processor.GetNextOutgoingQueueId();

                Message message;
                while (_outgoingMessages.TryDequeue(out message))
                    Resources.Processor.EnqueueOutgoing(message, message.Priority == MessagePriority.High ? -1 : queueId);
            }

            if (_incomingMessages.Count > 0)
            {
                var queueId = Resources.Processor.GetNextIncomingQueueId();

                Message message;
                while (_incomingMessages.TryDequeue(out message))
                    Resources.Processor.EnqueueIncoming(message, message.Priority == MessagePriority.High ? -1 : queueId);
            }
        }

        public void Dispose()
        {
            if (!this.Disposed)
            {
                this.Disposed = true;
                this.State = State.DISCONNECTED;

                this.Chat?.Quit(this);

                if (this.Account != null)
                {
                    if (this.Account.Battle != null)
                    {
                        if (!this.Account.Battle.Ended)
                        {
                            if (this.Account.Battle.Attacker == this.Account.Home.Level)
                            {
                                this.Account.Battle.EndBattleAsync();
                                this.Account.Battle = null;
                            }
                        }
                        else
                        {
                            this.Account.Battle = null;
                        }
                    }

                    if (this.Account.Player != null)
                    {
                        Resources.Accounts.SavePlayer(this.Account.Player);

                        /*
                        if (this.GameMode?.Level != null && this.Account.Player.BattleIdV2 > 0)
                        {
                            Resources.BattlesV2.Dequeue(this.GameMode.Level);
                        }
                        */

                        if (this.Account.Player.Alliance != null)
                        {
                            Player _;
                            long id = ((long)this.Account.HighId) << 32 | (uint)this.Account.LowId;

                            this.Account.Player.Alliance.DecrementTotalConnected();
                            this.Account.Player.Alliance.Members.Connected.TryRemove(id, out _);
                        }
                    }

                    if (this.Account.Home != null)
                    {
                        Resources.Accounts.SaveHome(this.Account.Home);
                    }

                    this.Account.Device = null;
                }

                if (this.GameMode?.CommandManager != null)
                {
                    if (this.GameMode.CommandManager.ServerCommands != null)
                    {
                        foreach (Command Command in this.GameMode.CommandManager.ServerCommands.Values.ToArray())
                        {
                            Command.Execute();
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "CommandManager != null but ServerCommands == null");
                    }
                }

                /*
                Account = null;
                Chat = null;
                GameMode = null;
                */
                ReceiveEncrypter = null;
                SendEncrypter = null;
            }
        }

        internal void Process(byte[] buffer, bool flush = true)
        {
            if (this.State != State.DISCONNECTED)
            {
                if (buffer.Length >= 7)
                {
                    int messageType = buffer[1] | (buffer[0] << 8);
                    int messageLength = buffer[4] | (buffer[3] << 8) | (buffer[2] << 16);
                    int messageVersion = buffer[6] | (buffer[5] << 8);

                    if (messageLength < 0x800000)
                    {
                        if (buffer.Length - 7 >= messageLength)
                        {
                            byte[] messageBytes = new byte[messageLength];
                            Array.Copy(buffer, 7, messageBytes, 0, messageLength);

                            if (this.ReceiveEncrypter != null)
                            {
                                messageBytes = this.ReceiveEncrypter.Decrypt(messageBytes);
                            }
                            else
                            {
                                if (messageType == 10101)
                                {
                                    this.UseRC4 = true;
                                    this.InitRC4Encrypters(Factory.RC4Key, "nonce");

                                    messageBytes = this.ReceiveEncrypter.Decrypt(messageBytes);
                                }
                            }

                            if (messageBytes != null)
                            {
                                Message message = Factory.CreateMessage((short)messageType, this, new Reader(messageBytes));

                                if (message != null)
                                {
                                    message.Version = (short)messageVersion;
                                    message.Timer = new Stopwatch();
                                    EnqueueIncomingMessage(message);
                                }

                                /*else
                                {
                                    File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\Dumps\\" + $"Unknown Message ({messageType}) - UserId ({(this.GameMode?.Level?.Player != null ? this.GameMode.Level.Player.HighID + "-" + this.GameMode.Level.Player.LowID : "-")}) - {DateTime.Now:yy_MM_dd__hh_mm_ss}.bin", messageBytes);

                                }*/
                            }
                            else
                            {
                                Logging.Error(this.GetType(), "Unable to decrypt message type " + messageType + ". Encrypter: " + this.ReceiveEncrypter + ".");
                            }

                            this.Token.Packet.RemoveRange(0, messageLength + 7);

                            if (buffer.Length - 7 - messageLength >= 7)
                            {
                                byte[] nextPacket = new byte[buffer.Length - 7 - messageLength];
                                Array.Copy(buffer, messageLength + 7, nextPacket, 0, nextPacket.Length);
                                this.Process(nextPacket, false);
                            }

                            if (flush)
                                Flush();
                        }
                    }
                    else
                    {
                        Resources.Gateway.Disconnect(this.Token.Args);
                    }
                }
            }
        }

        internal void InitRC4Encrypters(string key, string nonce)
        {
            this.ReceiveEncrypter = new RC4Encrypter(key, nonce);
            this.SendEncrypter = new RC4Encrypter(key, nonce);
        }

        internal struct DeviceInfo
        {
            internal bool Android;
            internal bool Advertising;

            internal string UDID;
            internal string OpenUDID;
            internal string AndroidID;
            internal string DeviceModel;
            internal string ADID;
            internal string OSVersion;
            internal string PreferredLanguage;
            internal string[] ClientVersion;

            internal int Locale;

            /*
            internal void ShowValues()
            {
                foreach (var Field in GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    if (Field != null)
                        Logging.Info(GetType(),
                            ConsolePad.Padding(Field.Name) + " : " +
                            ConsolePad.Padding(!string.IsNullOrEmpty(Field.Name) ? (Field.GetValue(this) != null ? Field.GetValue(this).ToString() : "(null)") : "(null)", 40));
            }
            */
        }
    }
}