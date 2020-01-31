namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Battles;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class BattleReportStreamEntry : AvatarStreamEntry
    {
        internal int StreamType;
        internal int MajorVersion;
        internal int BuildVersion;
        internal int ContentVersion;
        internal int ReplayShardId;

        internal bool IsRevengeUsed;

        internal long ReplayId;
        
        internal string BattleLogJSON;

        public BattleReportStreamEntry()
        {
        }

        public BattleReportStreamEntry(int streamType)
        {
            this.StreamType = streamType;
        }

        public BattleReportStreamEntry(Player player, Battle battle, long replayId, int streamType) : base(player)
        {
            this.MajorVersion = 9;
            this.BuildVersion = 256;
            this.ContentVersion = 12;
            this.ReplayId = replayId;
            this.StreamType = streamType;
            this.BattleLogJSON = battle.BattleLog.Save().ToString(Formatting.None);
        }

        internal override AvatarStream Type
        {
            get
            {
                return (AvatarStream) this.StreamType;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);

            Packet.AddString(this.BattleLogJSON);
            Packet.AddBool(this.IsRevengeUsed);
            Packet.AddInt(this.MajorVersion);
            Packet.AddInt(this.BuildVersion);
            Packet.AddInt(this.ContentVersion);

            Packet.AddBool(this.ReplayId != 0);

            if (this.ReplayId != 0)
            {
                Packet.AddInt(this.ReplayShardId);
                Packet.AddLong(this.ReplayId);
            }
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);
            JsonHelper.GetJsonString(Json, "log", out this.BattleLogJSON);
            JsonHelper.GetJsonBoolean(Json, "revengeUsed", out this.IsRevengeUsed);
            JsonHelper.GetJsonNumber(Json, "majorVersion", out this.MajorVersion);
            JsonHelper.GetJsonNumber(Json, "buildVersion", out this.BuildVersion);
            JsonHelper.GetJsonNumber(Json, "contentVersion", out this.ContentVersion);
            JsonHelper.GetJsonNumber(Json, "replayShardId", out this.ReplayShardId);
            JsonHelper.GetJsonNumber(Json, "replayId", out this.ReplayId);
        }

        internal override JObject Save()
        {
            JObject json = base.Save();

            json.Add("log", this.BattleLogJSON);
            json.Add("revengeUsed", this.IsRevengeUsed);
            json.Add("majorVersion", this.MajorVersion);
            json.Add("buildVersion", this.BuildVersion);
            json.Add("contentVersion", this.ContentVersion);

            if (this.ReplayId != 0)
            {
                json.Add("replayShardId", this.ReplayShardId);
                json.Add("replayId", this.ReplayId);
            }

            return json;
        }
    }
}