namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class AllianceKickOutStreamEntry : AvatarStreamEntry
    {
        internal Alliance Alliance;

        internal int AllianceHighId;
        internal int AllianceLowId;

        internal string Message;

        public AllianceKickOutStreamEntry()
        {
        }

        public AllianceKickOutStreamEntry(Player Player, Alliance Alliance) : base(Player)
        {
            this.Alliance = Alliance;
            this.AllianceHighId = Alliance.HighId;
            this.AllianceLowId = Alliance.LowId;
        }

        internal override AvatarStream Type
        {
            get
            {
                return AvatarStream.KickedFromClan;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);

            Packet.AddString(this.Message);
            Packet.AddInt(this.AllianceHighId);
            Packet.AddInt(this.AllianceLowId);
            Packet.AddString(this.Alliance.Header.Name);
            Packet.AddInt(this.Alliance.Header.Badge);
            Packet.AddBool(true);
            Packet.AddInt(this.SenderHighId);
            Packet.AddInt(this.SenderLowId);
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