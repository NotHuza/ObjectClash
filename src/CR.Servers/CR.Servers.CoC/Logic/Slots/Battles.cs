namespace CR.Servers.CoC.Logic.Slots
{
    using System.Collections.Concurrent;
    using System.Threading;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Database;
    using CR.Servers.CoC.Logic.Battles;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using System.Threading.Tasks;

    internal class Battles : ConcurrentDictionary<long, Core.Database.Models.Mongo.Battles>
    {
        private int Seed;

        internal Battles()
        {
            this.Seed = Mongo.BattleSeed;
        }

        internal void Add(Core.Database.Models.Mongo.Battles battle)
        {
            if (!this.ContainsKey(((long)battle.HighId << 32) | (uint)battle.LowId))
            {
                if (!this.TryAdd(((long)battle.HighId << 32) | (uint)battle.LowId, battle))
                {
                    Logging.Error(this.GetType(), "Unable to add the battle replay " + battle.HighId + "-" + battle.LowId + " in list.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to add the battle replay " + battle.HighId + "-" + battle.LowId + ". The player account is already in the dictionary.");
            }
        }

        internal async Task<Core.Database.Models.Mongo.Battles> Save(Battle battle)
        {
            Core.Database.Models.Mongo.Battles battleSave = new Core.Database.Models.Mongo.Battles
            {
                HighId = 0,
                LowId = Interlocked.Increment(ref this.Seed),
                Replay = battle.Recorder.Save().ToString(Formatting.None)
            };

            await Mongo.Battles.InsertOneAsync(battleSave);

            return battleSave;
        }

        internal Core.Database.Models.Mongo.Battles Get(long replayId)
        {
            Core.Database.Models.Mongo.Battles replay;
            if (!this.TryGetValue(replayId, out replay))
            {
                replay = Mongo.Battles.Find(T => T.HighId == (int) (replayId >> 32) && T.LowId == (int) replayId).Limit(1).SingleOrDefault();
            }

            return replay;
        }
    }
}