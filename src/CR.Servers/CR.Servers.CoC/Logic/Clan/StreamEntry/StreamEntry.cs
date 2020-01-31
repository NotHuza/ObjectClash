namespace CR.Servers.CoC.Logic.Clan
{
    using System;
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    [JsonConverter(typeof(StreamEntryConverter))]
    internal class StreamEntry
    {
        internal DateTime Created = DateTime.UtcNow;

        internal int HighId;
        internal int LowId;
        internal long RequesterId;

        internal int SenderHighId;
        internal int SenderLeague;
        internal int SenderLevel;
        internal int SenderLowId;

        internal string SenderName;

        internal Role SenderRole;

        public StreamEntry()
        {
            // StreamEntry.
        }

        public StreamEntry(Member Member)
        {
            this.SenderHighId = Member.Player.HighID;
            this.SenderLowId = Member.Player.LowID;

            this.SenderName = Member.Player.Name;

            this.SenderLevel = Member.Player.ExpLevel;
            this.SenderRole = Member.Role;
            this.SenderLeague = Member.Player.League;
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

        internal virtual AllianceStream Type
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

            Packet.AddByte(3);

            Packet.AddInt(this.SenderHighId);
            Packet.AddInt(this.SenderLowId);
            Packet.AddInt(this.SenderHighId);
            Packet.AddInt(this.SenderLowId);


            Packet.AddString(this.SenderName);

            Packet.AddInt(this.SenderLevel);
            Packet.AddInt(this.SenderLeague);
            Packet.AddInt((int) this.SenderRole);

            Packet.AddInt(this.Age);
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

                int Role;
                JsonHelper.GetJsonNumber(Base, "sender_role", out Role);
                JsonHelper.GetJsonDateTime(Base, "date", out this.Created);

                this.SenderRole = (Role) Role;
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
                {"sender_role", (int) this.SenderRole},
                {"date", this.Created}
            };

            JObject Json = new JObject
            {
                {"type", (int) this.Type},
                {"base", Base}
            };

            return Json;
        }
    }

    internal class StreamEntryConverter : JsonConverter
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
            return Type.BaseType == typeof(StreamEntry) || Type == typeof(StreamEntry);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject Token = JObject.Load(reader);

            int Type;
            if (JsonHelper.GetJsonNumber(Token, "type", out Type))
            {
                StreamEntry Entry;

                switch (Type)
                {
                    case 1:
                        Entry = new DonateStreamEntry();
                        break;
                    case 2:
                        Entry = new ChatStreamEntry();
                        break;
                    case 3:
                        Entry = new JoinRequestStreamEntry();
                        break;
                    case 4:
                        Entry = new EventStreamEntry();
                        break;
                    case 16:
                        Entry = new GiftStreamEntry();
                        break;
                    default:
                        Entry = new StreamEntry();
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
            StreamEntry StreamEntry = (StreamEntry) value;

            if (StreamEntry != null)
            {
                StreamEntry.Save().WriteTo(writer);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}