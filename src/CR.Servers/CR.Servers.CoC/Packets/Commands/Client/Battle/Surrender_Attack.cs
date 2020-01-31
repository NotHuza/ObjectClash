namespace CR.Servers.CoC.Packets.Commands.Client.Battle
{
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;
    using Newtonsoft.Json.Linq;

    internal class Surrender_Attack : Command
    {
        public Surrender_Attack(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 703;
            }
        }

        internal override void Execute()
        {
            // Space
        }

        internal override void Load(JToken Token)
        {
            JObject Command = (JObject) Token["c"];

            if (Command != null)
            {
                JsonHelper.GetJsonNumber(Command, "t", out this.ExecuteSubTick);
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
                        {"t", this.ExecuteSubTick}
                    }
                }
            };

            return Json;
        }
    }
}