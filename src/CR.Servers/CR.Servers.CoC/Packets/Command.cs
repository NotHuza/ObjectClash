using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CR.Servers.CoC.Core;
using CR.Servers.CoC.Logic;
using CR.Servers.Extensions;
using CR.Servers.Extensions.Binary;
using CR.Servers.Extensions.List;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace CR.Servers.CoC.Packets
{
    public class Command
    {
        private static readonly Task s_completedTask = Task.FromResult<object>(null);

        internal Device Device;
        internal int ExecuteSubTick = -1;

        internal Reader Reader;

        public Command()
        {
            // Space
        }

        public Command(Device Device)
        {
            this.Device = Device;
        }

        public Command(Device Device, Reader Reader)
        {
            this.Device = Device;
            this.Reader = Reader;
        }

        internal virtual int Type
        {
            get
            {
                return 0;
            }
        }

        internal virtual bool IsServerCommand
        {
            get
            {
                return false;
            }
        }

        internal virtual void Decode()
        {
            this.ExecuteSubTick = this.Reader.ReadInt32();
        }

        internal virtual void Encode(List<byte> Data)
        {
            Data.AddInt(this.ExecuteSubTick);
        }

        internal virtual void Execute()
        {
            // Space
        }

        internal virtual Task ExecuteAsync()
        {
            return s_completedTask;
        }

        internal virtual void Load(JToken Token)
        {
        }

        internal virtual JObject Save()
        {
            return null;
        }

        internal JObject SaveBase()
        {
            return new JObject
            {
                {
                    "t", this.ExecuteSubTick
                }
            };
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
                {
                    Logging.Info(this.GetType(), ConsolePad.Padding(Field.Name) + " : " + ConsolePad.Padding(!string.IsNullOrEmpty(Field.Name) ? (Field.GetValue(this) != null ? Field.GetValue(this).ToString() : "(null)") : "(null)", 40));
                }
            }
        }

        internal void Log()
        {
            File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\Dumps\\" + $"{this.GetType().Name} ({this.Type}) - {DateTime.Now:yy_MM_dd__hh_mm_ss}.bin", this.Reader.ReadBytes((int)(this.Reader.BaseStream.Length - this.Reader.BaseStream.Position)));
        }
    }
}