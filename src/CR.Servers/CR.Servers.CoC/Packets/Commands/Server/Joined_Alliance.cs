namespace CR.Servers.CoC.Packets.Commands.Server
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;

    internal class Joined_Alliance : ServerCommand
    {
        internal int AllianceBadge;

        internal long AllianceId;
        internal int AllianceLevel;

        internal string AllianceName;

        internal bool CreateAlliance;

        public Joined_Alliance(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        public Joined_Alliance(Device Device) : base(Device)
        {
        }

        internal override int Type
        {
            get
            {
                return 1;
            }
        }

        internal override void Decode()
        {
            this.AllianceId = this.Reader.ReadInt64();
            this.AllianceName = this.Reader.ReadString();
            this.AllianceBadge = this.Reader.ReadInt32();
            this.CreateAlliance = this.Reader.ReadBoolean();
            this.AllianceLevel = this.Reader.ReadInt32();

            base.Decode();
        }

        internal override void Encode(List<byte> Packet)
        {
            Packet.AddLong(this.AllianceId);
            Packet.AddString(this.AllianceName);
            Packet.AddInt(this.AllianceBadge);
            Packet.AddBool(this.CreateAlliance);
            Packet.AddInt(this.AllianceLevel);

            base.Encode(Packet);
        }
    }
}