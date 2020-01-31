using System;
using CR.Servers.CoC.Logic;
using CR.Servers.Extensions.Binary;

namespace CR.Servers.CoC.Packets.Messages.Client.Home
{
    internal class KeepAliveMessage : Message
    {
        public KeepAliveMessage(Device Device, Reader Reader) : base(Device, Reader)
        {
            // Space
        }

        public override MessagePriority Priority
        {
            get
            {
                return MessagePriority.High;
            }
        }

        internal override short Type
        {
            get
            {
                return 10108;
            }
        }

        internal override void Process()
        {
            Device.KeepAliveServerMessage.Send();
            Device.LastKeepAlive = DateTime.UtcNow;
        }
    }
}