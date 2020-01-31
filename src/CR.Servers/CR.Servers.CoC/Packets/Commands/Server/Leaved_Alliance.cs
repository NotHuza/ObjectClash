namespace CR.Servers.CoC.Packets.Commands.Server
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;

    internal class Leaved_Alliance : ServerCommand
    {
        internal long AllianceId;

        public Leaved_Alliance(Device Device) : base(Device)
        {
        }

        public Leaved_Alliance(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 2;
            }
        }

        internal override void Decode()
        {
            this.AllianceId = this.Reader.ReadInt64();
            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddLong(this.AllianceId);
            base.Encode(Data);
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            Level.Player.AllianceHighId = 0;
            Level.Player.AllianceLowId = 0;
            Level.Player.AllianceMember = null;
            Level.Player.Alliance = null;
        }
    }
}