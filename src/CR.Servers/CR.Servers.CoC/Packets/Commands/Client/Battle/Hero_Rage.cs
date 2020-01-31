namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class Hero_Rage : Command
    {
        internal int GlobalId;

        public Hero_Rage(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 706;
            }
        }

        internal override void Decode()
        {
            this.GlobalId = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddInt(this.GlobalId);
            base.Encode(Data);
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
        }


        internal override JObject Save()
        {
            JObject Json = new JObject
            {
                {"base", this.SaveBase()},
                {"d", this.GlobalId}
            };

            return Json;
        }
    }
}