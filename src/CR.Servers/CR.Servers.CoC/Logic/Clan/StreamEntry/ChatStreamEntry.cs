namespace CR.Servers.CoC.Logic.Clan
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class ChatStreamEntry : StreamEntry
    {
        internal string Message;

        public ChatStreamEntry()
        {
        }

        public ChatStreamEntry(Member Member) : base(Member)
        {
        }

        internal override AllianceStream Type
        {
            get
            {
                return AllianceStream.Chat;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);
            Packet.AddString(this.Message);
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);

            var Message = (string)null;
            if (JsonHelper.GetJsonString(Json, "message", out Message))
            {
                this.Message = Message ?? string.Empty;
            }
            else
            {
                this.Message = string.Empty;
            }
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();
            Json.Add("message", this.Message);
            return Json;
        }
    }
}