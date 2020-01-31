namespace CR.Servers.CoC.Packets.Commands.Server
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;

    internal class Alliance_Unit_Received : ServerCommand
    {
        internal string Donator;
        internal int Level;
        internal int UnitId;
        internal int UnitType;

        public Alliance_Unit_Received(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        public Alliance_Unit_Received(Device Device) : base(Device)
        {
        }

        internal override int Type
        {
            get
            {
                return 5;
            }
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddString(this.Donator);
            Data.AddInt(this.UnitType);
            Data.AddInt(this.UnitId);
            Data.AddInt(this.Level);
            base.Encode(Data);
        }

        internal override void Decode()
        {
            this.Donator = this.Reader.ReadString();
            this.UnitType = this.Reader.ReadInt32();
            this.UnitId = this.Reader.ReadInt32();
            this.Level = this.Reader.ReadInt32();
            base.Decode();
        }
    }
}