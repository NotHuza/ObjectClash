namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class AllianceMailAvatarStreamEntry : AvatarStreamEntry
    {
        internal Alliance Alliance;

        internal int AllianceHighId;
        internal int AllianceLowId;

        internal string Message;

        public AllianceMailAvatarStreamEntry()
        {
        }

        public AllianceMailAvatarStreamEntry(Player Player) : base(Player)
        {
        }

        public AllianceMailAvatarStreamEntry(Player Player, Alliance Alliance) : base(Player)
        {
            this.Alliance = Alliance;
            this.AllianceHighId = Alliance.HighId;
            this.AllianceLowId = Alliance.LowId;
        }

        internal override AvatarStream Type
        {
            get
            {
                return AvatarStream.ClanMail;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);
            Packet.AddString(this.Message);
            Packet.AddBool(true);
            Packet.AddInt(this.SenderHighId);
            Packet.AddInt(this.SenderLowId);
            Packet.AddInt(this.AllianceHighId);
            Packet.AddInt(this.AllianceLowId);
            Packet.AddString(this.Alliance != null ? this.Alliance.Header.Name : "[System] Command Manager");
            Packet.AddInt(this.Alliance != null ? this.Alliance.Header.Badge : 1526733402);
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);

            JsonHelper.GetJsonString(Json, "message", out this.Message);

            if (JsonHelper.GetJsonNumber(Json, "alliance_high_id", out this.AllianceHighId) && JsonHelper.GetJsonNumber(Json, "alliance_low_id", out this.AllianceLowId))
            {
                this.Alliance = Resources.Clans.Get(this.AllianceHighId, this.AllianceLowId);
            }
            else
            {
                Logging.Error(this.GetType(), $"Unable to load Alliance for ClanMailEntry. UserID {this.SenderHighId} - {this.SenderLowId} with EntryId {this.HighId} - {this.LowId} ");
            }
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();
            Json.Add("message", this.Message);
            Json.Add("alliance_high_id", this.AllianceHighId);
            Json.Add("alliance_low_id", this.AllianceLowId);
            return Json;
        }
    }
}