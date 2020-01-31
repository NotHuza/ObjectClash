namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class Place_Attacker : Command
    {
        internal CharacterData Character;

        internal int X;
        internal int Y;

        public Place_Attacker()
        {
        }

        public Place_Attacker(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 700;
            }
        }

        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();
            this.Character = this.Reader.ReadData<CharacterData>();
            base.Decode();
        }

        internal override void Encode(List<byte> Data)
        {
            Data.AddInt(this.X);
            Data.AddInt(this.Y);
            Data.AddInt(this.Character.GlobalId);
            base.Encode(Data);
        }

        internal override void Execute()
        {
            if (this.Character != null)
            {
                Level Level = this.Device.GameMode.Level;

                if (Level.GameObjectManager.Map == 0)
                {
                    Item Unit = Level.Player.Units.GetByData(this.Character);

                    if (Unit != null)
                    {
                        if (Unit.Count > 0)
                        {
                            Unit.Count--;
                        }
                    }
                }
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

                JsonHelper.GetJsonData(Command, "d", out this.Character);
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
                        {"d", this.Character.GlobalId},
                        {"x", this.X},
                        {"y", this.Y}
                    }
                }
            };

            return Json;
        }
    }
}