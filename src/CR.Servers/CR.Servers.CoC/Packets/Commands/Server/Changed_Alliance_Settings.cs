namespace CR.Servers.CoC.Packets.Commands.Server
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;

    internal class Changed_Alliance_Settings : ServerCommand
    {
        internal int AllianceBadge;


        internal long AllianceId;

        public Changed_Alliance_Settings(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        public Changed_Alliance_Settings(Device Device) : base(Device)
        {
        }

        internal override int Type
        {
            get
            {
                return 6;
            }
        }

        internal override void Decode()
        {
            this.AllianceId = this.Reader.ReadInt64();
            this.AllianceBadge = this.Reader.ReadInt32();

            base.Decode();
        }

        internal override void Encode(List<byte> Packet)
        {
            Packet.AddLong(this.AllianceId);
            Packet.AddInt(this.AllianceBadge);

            base.Encode(Packet);
        }
    }
}