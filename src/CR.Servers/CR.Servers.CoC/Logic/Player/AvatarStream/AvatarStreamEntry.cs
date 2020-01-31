namespace CR.Servers.CoC.Logic
{
    using System;
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [JsonConverter(typeof(AvatarStreamEntryConverter))]
    internal class AvatarStreamEntry
    {
        internal DateTime Created = DateTime.UtcNow;
        internal int HighId;
        internal int LowId;

        internal byte New = 2;

        internal int SenderHighId;
        internal int SenderLeague;
        internal int SenderLevel;
        internal int SenderLowId;

        internal string SenderName;

        public AvatarStreamEntry()
        {
            // AvatarStreamEntry.
        }

        public AvatarStreamEntry(Player Player)
        {
            this.SenderHighId = Player.HighID;
            this.SenderLowId = Player.LowID;

            this.SenderName = Player.Name;

            this.SenderLevel = Player.ExpLevel;
            this.SenderLeague = Player.League;
        }

        internal int Age
        {
            get
            {
                return (int) DateTime.UtcNow.Subtract(this.Created).TotalSeconds;
            }
        }

        internal long StreamId
        {
            get
            {
                return ((long) this.HighId << 32) | (uint) this.LowId;
            }
        }

        internal virtual AvatarStream Type
        {
            get
            {
                Logging.Error(this.GetType(), "Type must be overrided");
                return 0;
            }
        }

        internal virtual void Encode(List<byte> Packet)
        {
            Packet.AddInt((int) this.Type);
            Packet.AddInt(this.HighId);
            Packet.AddInt(this.LowId);

            Packet.AddBool(true);
            {
                Packet.AddInt(this.SenderHighId);
                Packet.AddInt(this.SenderLowId);
            }

            Packet.AddString(this.SenderName);
            Packet.AddInt(this.SenderLevel);
            Packet.AddInt(this.SenderLeague);

            Packet.AddInt(this.Age);
            Packet.AddByte(this.New);
        }

        internal virtual void Load(JToken Json)
        {
            JToken Base = Json["base"];

            if (Base != null)
            {
                JsonHelper.GetJsonNumber(Base, "high_id", out this.HighId);
                JsonHelper.GetJsonNumber(Base, "low_id", out this.LowId);

                JsonHelper.GetJsonNumber(Base, "sender_high_id", out this.SenderHighId);
                JsonHelper.GetJsonNumber(Base, "sender_low_id", out this.SenderLowId);
                JsonHelper.GetJsonNumber(Base, "sender_lvl", out this.SenderLevel);

                JsonHelper.GetJsonString(Base, "sender_name", out this.SenderName);

                if (this.SenderName == null)
                {
                    Logging.Error(this.GetType(), "Load() - SenderName is NULL.");
                    this.SenderName = string.Empty;
                }

                JsonHelper.GetJsonDateTime(Base, "date", out this.Created);
                JsonHelper.GetJsonByte(Base, "new", out this.New);
            }
            else
            {
                Logging.Error(this.GetType(), "Json doesn't contains base object!");
            }
        }

        internal virtual JObject Save()
        {
            JObject Base = new JObject
            {
                {"high_id", this.HighId},
                {"low_id", this.LowId},
                {"sender_high_id", this.SenderHighId},
                {"sender_low_id", this.SenderLowId},
                {"sender_lvl", this.SenderLevel},
                {"sender_name", this.SenderName},
                {"date", this.Created},
                {"new", this.New}
            };

            JObject Json = new JObject
            {
                {"type", (int) this.Type},
                {"base", Base}
            };

            return Json;
        }
    }

    internal class AvatarStreamEntryConverter : JsonConverter
    {
        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override bool CanConvert(Type Type)
        {
            return Type.BaseType == typeof(AvatarStreamEntry) || Type == typeof(AvatarStreamEntry);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject Token = JObject.Load(reader);

            int Type;
            if (JsonHelper.GetJsonNumber(Token, "type", out Type))
            {
                AvatarStreamEntry Entry;

                switch (Type)
                {
                    case 2:
                        Entry = new BattleReportStreamEntry(2);
                        break;
                    case 5:
                        Entry = new AllianceKickOutStreamEntry();
                        break;
                    case 6:
                        Entry = new AllianceMailAvatarStreamEntry();
                        break;
                    case 7:
                        Entry = new BattleReportStreamEntry(7);
                        break;
                    default:
                        Entry = new AvatarStreamEntry();
                        break;
                }

                Entry.Load(Token);

                return Entry;
            }

            Logging.Info(this.GetType(), "ReadJson() - JsonObject doesn't contains 'type' key.");

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            AvatarStreamEntry Mail = (AvatarStreamEntry) value;

            if (Mail != null)
            {
                Mail.Save().WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}