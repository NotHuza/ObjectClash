namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class Place_Alliance_Portal : Command
    {
        internal int GlobalId;
        internal int X;
        internal int Y;

        public Place_Alliance_Portal()
        {
        }

        public Place_Alliance_Portal(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 701;
            }
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.GlobalId = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddInt(this.X);
            Data.AddInt(this.Y);
            Data.AddInt(this.GlobalId);
            base.Encode(Data);
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            if (Level.GameObjectManager.Map == 0)
            {
                /*if (!Level.Modes.IsAttackingOwnBase)
                {
                }*/
                Level.Player.AllianceUnits.Clear();
                Level.Player.CastleUsedCapacity = 0;
            }
        }

        internal override void Load(JToken Token)
        {
            JObject Command = (JObject) Token["c"];

            if (Command != null)
            {
                JObject TickBase = (JObject) Command["base"];

                if (TickBase != null)
                {
                    JsonHelper.GetJsonNumber(TickBase, "t", out this.ExecuteSubTick);
                }

                JsonHelper.GetJsonNumber(Command, "d", out this.GlobalId);
                JsonHelper.GetJsonNumber(Command, "x", out this.X);
                JsonHelper.GetJsonNumber(Command, "y", out this.Y);
            }
        }

        internal override JObject Save()
        {
            JObject Json = new JObject
            {
                {"ct", this.Type},
                {
                    "c", new JObject
                    {
                        {"base", this.SaveBase()},
                        {"d", this.GlobalId},
                        {"x", this.X},
                        {"y", this.Y}
                    }
                }
            };

            return Json;
        }
    }
}