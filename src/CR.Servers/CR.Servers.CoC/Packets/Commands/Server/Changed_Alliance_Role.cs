namespace CR.Servers.CoC.Packets.Commands.Server
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;

    internal class Changed_Alliance_Role : ServerCommand
    {
        internal long AllianceId;
        internal Role AllianceRole;

        public Changed_Alliance_Role(Device Device) : base(Device)
        {
        }

        public Changed_Alliance_Role(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 8;
            }
        }

        internal override void Decode()
        {
            this.AllianceId = this.Reader.ReadInt64();
            this.AllianceRole = (Role) this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddLong(this.AllianceId);
            Data.AddInt((int) this.AllianceRole);
            base.Encode(Data);
        }
    }
}