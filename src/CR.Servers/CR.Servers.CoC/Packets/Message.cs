using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Logic;
using CR.Servers.CoC.Packets.Messages.Server.Home;
using CR.Servers.Extensions;
using CR.Servers.Extensions.Binary;
using System.Threading.Tasks;

namespace CR.Servers.CoC.Packets
{
    public enum MessagePriority
    {
        Normal,
        High
    };

    public abstract class Message
    {
        private static readonly Task s_completedTask = Task.FromResult<object>(null);

        internal Stopwatch Timer;

        private readonly List<byte> _data;
        private readonly Device _device;
        private readonly Reader _reader;

        protected Message(Device device)
        {
            _device = device;
            _reader = null;
            _data = new List<byte>();
        }

        protected Message(Device device, Reader reader)
        {
            _device = device;
            _reader = reader;
        }

        public Reader Reader => _reader;
        public Device Device => _device;
        public List<byte> Data => _data;
        public short Version { get; set; }
        public virtual MessagePriority Priority => MessagePriority.Normal;
        internal abstract short Type { get; }

        internal virtual void Decode()
        {
            // Space
        }

        internal virtual void Encode()
        {
            // Space
        }

        internal virtual void Process()
        {
            // Space
        }

        internal virtual Task ProcessAsync()
        {
            /* Run the normal Process method. */
            Process();

            return s_completedTask;
        }

        protected virtual void SendChatMessage(string message)
        {
            new GlobalChatLineMessage(this.Device, this.Device.GameMode.Level.Player)
            {
                Message = message,
                Name = "[System] Command Manager",
                ExpLevel = 300,
                League = 22,
                Bot = true
            }.Send();
        }

        internal void ShowBuffer()
        {
            Logging.Info(this.GetType(), BitConverter.ToString(this.Reader.ReadBytes((int)(this.Reader.BaseStream.Length - this.Reader.BaseStream.Position))));
        }

        internal void ShowValues()
        {
            foreach (FieldInfo Field in this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (Field != null)
                    Logging.Info(this.GetType(), ConsolePad.Padding(Field.Name) + " : " + ConsolePad.Padding(!string.IsNullOrEmpty(Field.Name) ? (Field.GetValue(this) != null ? Field.GetValue(this).ToString() : "(null)") : "(null)", 40));
            }
        }

        internal void Send()
        {
            if (Device.Connected)
                Device.EnqueueOutgoingMessage(this);
        }

        internal void Log()
        {
            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\Dumps\\" + $"{this.GetType().Name} ({this.Type}) - UserId ({(this.Device.GameMode?.Level?.Player != null ? this.Device.GameMode.Level.Player.HighID + "-" + this.Device.GameMode.Level.Player.LowID : "-")}) - {DateTime.Now:yy_MM_dd__hh_mm_ss}.bin", this.Reader.ReadBytes((int)(this.Reader.BaseStream.Length - this.Reader.BaseStream.Position)));
        }
    }
}