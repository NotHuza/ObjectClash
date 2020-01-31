using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.CoC.Packets;
using CR.Servers.Extensions.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CR.Servers.CoC.Logic
{
    internal class CommandDebug
    {
        private readonly List<Command> _commands;
        private readonly Player _player;
        private JObject _startLevel;
        private JObject _endLevel;

        public CommandDebug(Player player)
        {
            _player = player;
            _commands = new List<Command>(256);
        }

        public void Capture()
        {
            _startLevel = _player.Level.Save();
            _endLevel = null;
            _commands.Clear();
        }

        public void Record(Command command)
        {
            _commands.Add(command);
        }

        public void Dump()
        {
            _endLevel = _player.Level.Save();

            var dump = new DebugDump
            {
                StartLevel = _startLevel,
                EndLevel = _endLevel,
                UserId = _player.UserId,
                Commands = _commands
            };

            var json = JsonConvert.SerializeObject(dump, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CommandContractResolver()
            });

            lock (_startLevel)
            {
                Directory.CreateDirectory("Dumps");
                File.WriteAllText("Dumps/" + _player.UserId + ".json", json);
            }
        }

        public class CommandContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                if (type == typeof(Command))
                {
                    var typePropertyInfo = type.GetProperty("Type", BindingFlags.Instance | BindingFlags.NonPublic);
                    var property = base.CreateProperty(typePropertyInfo, MemberSerialization.OptOut);
                    property.Readable = true;
                    property.Writable = true;

                    return new List<JsonProperty>
                    {
                        property
                    };
                }

                if (type.IsSubclassOf(typeof(Data)))
                {
                    var globalIdPropertyInfo = type.GetProperty("GlobalId", BindingFlags.Instance | BindingFlags.Public);
                    var property = base.CreateProperty(globalIdPropertyInfo, MemberSerialization.OptOut);
                    property.Readable = true;
                    property.Writable = true;

                    return new List<JsonProperty>
                    {
                        property
                    };
                }

                if (type.IsSubclassOf(typeof(Command)))
                {
                    var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
                    var properties = new List<JsonProperty>(fields.Length + 1);
                    foreach (var field in fields)
                    {
                        if (field.FieldType == typeof(Device) || field.FieldType == typeof(Reader))
                            continue;

                        var property = base.CreateProperty(field, MemberSerialization.OptOut);
                        property.Readable = true;
                        property.Writable = true;
                        properties.Add(property);
                    }

                    var typePropertyInfo = type.GetProperty("Type", BindingFlags.Instance | BindingFlags.NonPublic);
                    var typeProperty = base.CreateProperty(typePropertyInfo, MemberSerialization.OptOut);
                    typeProperty.Readable = true;
                    typeProperty.Writable = true;

                    properties.Add(typeProperty);
                    return properties;
                }

                return base.CreateProperties(type, memberSerialization);
            }
        }


        public class DebugDump
        {
            public long UserId;
            public JObject StartLevel;
            public JObject EndLevel;
            public List<Command> Commands;
        }
    }
}
