namespace CR.Servers.CoC.Logic.Clan
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class EventStreamEntry : StreamEntry
    {
        internal AllianceEvent Event;
        internal int ExecuterHighId;
        internal int ExecuterLowId;

        internal string ExecuterName;

        public EventStreamEntry()
        {
        }

        public EventStreamEntry(Member Member, Member Executer, AllianceEvent Event) : base(Member)
        {
            this.ExecuterHighId = Executer.HighId;
            this.ExecuterLowId = Executer.LowId;
            this.ExecuterName = Executer.Player.Name;

            this.Event = Event;
        }

        internal override AllianceStream Type
        {
            get
            {
                return AllianceStream.AllianceEvent;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);

            Packet.AddInt((int) this.Event);
            Packet.AddInt(this.ExecuterHighId);
            Packet.AddInt(this.ExecuterLowId);
            Packet.AddString(this.ExecuterName);
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);

            int Event;
            if (JsonHelper.GetJsonNumber(Json, "event", out Event))
            {
                this.Event = (AllianceEvent) Event;
            }

            JsonHelper.GetJsonNumber(Json, "exc_id_high", out this.ExecuterHighId);
            JsonHelper.GetJsonNumber(Json, "exc_id_low", out this.ExecuterLowId);
            JsonHelper.GetJsonString(Json, "exc_name", out this.ExecuterName);
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();

            Json.Add("event", (int) this.Event);
            Json.Add("exc_id_high", this.ExecuterHighId);
            Json.Add("exc_id_low", this.ExecuterLowId);
            Json.Add("exc_name", this.ExecuterName);

            return Json;
        }
    }
}