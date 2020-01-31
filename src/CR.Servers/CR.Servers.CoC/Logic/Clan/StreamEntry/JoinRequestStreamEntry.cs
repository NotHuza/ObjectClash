namespace CR.Servers.CoC.Logic.Clan
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class JoinRequestStreamEntry : StreamEntry
    {
        internal string Judge;

        internal string Message;
        internal InviteState State = InviteState.Waiting;

        public JoinRequestStreamEntry()
        {
        }

        public JoinRequestStreamEntry(Member Member) : base(Member)
        {
        }

        internal override AllianceStream Type
        {
            get
            {
                return AllianceStream.JoinRequest;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);
            Packet.AddString(this.Message);
            Packet.AddString(this.Message);
            Packet.AddInt((int) this.State);
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);

            string Message;
            if (JsonHelper.GetJsonString(Json, "message", out Message))
            {
                this.Message = Message ?? string.Empty;
            }
            else
            {
                this.Message = string.Empty;
            }

            string Judge;
            if (JsonHelper.GetJsonString(Json, "judge", out Judge))
            {
                this.Judge = Judge ?? string.Empty;
            }
            else
            {
                this.Judge = string.Empty;
            }

            int State;
            if (JsonHelper.GetJsonNumber(Json, "state", out State))
            {
                this.State = (InviteState) State;
            }
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();
            Json.Add("message", this.Message);
            Json.Add("judge", this.Judge);
            Json.Add("state", (int) this.State);
            return Json;
        }
    }
}