namespace CR.Servers.CoC.Logic
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Network;
    using CR.Servers.CoC.Packets.Messages.Server.Avatar;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json;

    internal class Inbox
    {
        [JsonProperty] internal ConcurrentDictionary<long, AvatarStreamEntry> Entries;
        internal Player Player;

        [JsonProperty] internal int Seed;

        public Inbox(Player Player)
        {
            this.Player = Player;
            this.Entries = new ConcurrentDictionary<long, AvatarStreamEntry>();
        }

        internal void Add(AvatarStreamEntry StreamEntry)
        {
            if ((int) StreamEntry.Type != -1)
            {
                StreamEntry.LowId = Interlocked.Increment(ref this.Seed);

                if (this.Entries.TryAdd(StreamEntry.StreamId, StreamEntry))
                {
                    if (this.Entries.Count > 50)
                    {
                        AvatarStreamEntry Removed;
                        if (this.Entries.TryRemove(this.Entries.Keys.First(), out Removed))
                        {
                            this.RemoveEntry(Removed);
                        }
                    }

                    if (this.Player.Connected)
                    {
                        new AvatarStreamEntryMessage(this.Player.Level.GameMode.Device) {StreamEntry = StreamEntry}.Send();
                    }
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Impossible to add a base of AvatarStreamEntry.");
            }
        }

        internal void Remove(AvatarStreamEntry MailEntry)
        {
            AvatarStreamEntry Removed;
            if (this.Entries.TryRemove(MailEntry.StreamId, out Removed))
            {
                this.RemoveEntry(Removed);
            }
        }

        internal void Remove(int StreamId)
        {
            AvatarStreamEntry Removed;
            if (this.Entries.TryRemove(StreamId, out Removed))
            {
                this.RemoveEntry(Removed);
            }
        }

        internal void RemoveEntry(AvatarStreamEntry StreamEntry)
        {
            if (this.Player.Connected)
            {
                //new Avatar_Stream_Entry_Removed_Message(this.Player.Device, StreamEntry).Send();
            }
        }

        internal void Update(AvatarStreamEntry StreamEntry)
        {
            if (this.Entries.ContainsKey(StreamEntry.StreamId))
            {
                this.Entries[StreamEntry.StreamId] = StreamEntry;

                if (this.Player.Connected)
                {
                    new AvatarStreamEntryMessage(this.Player.Level.GameMode.Device) {StreamEntry = StreamEntry}.Send();
                }
            }
        }

        internal void Encode(List<byte> Packet)
        {
            AvatarStreamEntry[] Streams = this.Entries.Values.ToArray();
            Packet.AddInt(Streams.Length);

            foreach (AvatarStreamEntry Stream in Streams)
            {
                Stream.Encode(Packet);
            }
        }


        internal void Tick()
        {
            foreach (AvatarStreamEntry StreamEntry in this.Entries.Values.ToArray())
            {
                if (StreamEntry.Age >= 86400 * 30)
                {
                    this.Remove(StreamEntry);
                }
            }
        }
    }
}