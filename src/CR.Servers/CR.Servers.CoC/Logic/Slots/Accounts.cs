namespace CR.Servers.CoC.Logic.Slots
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Core.Database;
    using CR.Servers.CoC.Core.Database.Models.Mongo;
    using CR.Servers.CoC.Files;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    internal class Accounts : ConcurrentDictionary<long, WeakReference<Account>>
    {
        private readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None
        };

        internal ConcurrentDictionary<long, WeakReference<Home>> Homes;
        internal ConcurrentDictionary<long, WeakReference<Player>> Players;
        private static Thread _savePlayerThread;
        private static Thread _saveHomeThread;
        private static ConcurrentQueue<Player> _savePlayerQueue;
        private static ConcurrentQueue<Home> _saveHomeQueue;

        private int _seed;

        internal Accounts()
        {
            this._seed = Mongo.PlayerSeed;

            this.Homes = new ConcurrentDictionary<long, WeakReference<Home>>();
            this.Players = new ConcurrentDictionary<long, WeakReference<Player>>();

            _savePlayerQueue = new ConcurrentQueue<Player>();
            _saveHomeQueue = new ConcurrentQueue<Home>();
            _savePlayerThread = new Thread(SavePlayerTask)
            {

                Priority = ThreadPriority.Highest
            };
            _saveHomeThread = new Thread(SaveHomeTask)
            {
                Priority = ThreadPriority.Highest
            };
            _savePlayerThread.Start();
            _saveHomeThread.Start();
        }

        internal void Add(Player Player)
        {
            if (!this.Players.ContainsKey(Player.UserId))
            {
                if (!this.Players.TryAdd(Player.UserId, new WeakReference<Player>(Player)))
                {
                    Logging.Error(this.GetType(), "Unable to add the player avatar " + Player.HighID + "-" + Player.LowID + " in list.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to add the player avatar " + Player.HighID + "-" + Player.LowID + ". The player avatar is already in the dictionary.");
            }
        }

        internal void Add(Home Home)
        {
            if (!this.Homes.ContainsKey(((long)Home.HighID << 32) | (uint)Home.LowID))
            {
                if (!this.Homes.TryAdd(((long)Home.HighID << 32) | (uint)Home.LowID, new WeakReference<Home>(Home)))
                {
                    Logging.Error(this.GetType(), "Unable to add the player home " + Home.HighID + "-" + Home.LowID + " in list.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to add the player home " + Home.HighID + "-" + Home.LowID + ". The player home is already in the dictionary.");
            }
        }

        internal void Add(Account account)
        {
            if (!this.ContainsKey(((long)account.HighId << 32) | (uint)account.LowId))
            {
                if (!this.TryAdd(((long)account.HighId << 32) | (uint)account.LowId, new WeakReference<Account>(account)))
                {
                    Logging.Error(this.GetType(), "Unable to add the player account " + account.HighId + "-" + account.LowId + " in list.");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to add the player account " + account.HighId + "-" + account.LowId + ". The player account is already in the dictionary.");
            }
        }

        private bool TryGetAccount(long id, out Account account)
        {
            WeakReference<Account> tmpRef;
            if (TryGetValue(id, out tmpRef))
            {
                if (tmpRef.TryGetTarget(out account))
                    return true;

                TryRemove(id, out tmpRef);
                return false;
            }

            account = null;
            return false;
        }

        private bool TryGetHome(long id, out Home home)
        {
            WeakReference<Home> tmpRef;
            if (Homes.TryGetValue(id, out tmpRef))
            {
                if (tmpRef.TryGetTarget(out home))
                    return true;

                Homes.TryRemove(id, out tmpRef);
                return false;
            }

            home = null;
            return false;
        }

        private bool TryGetPlayer(long id, out Player player)
        {
            WeakReference<Player> tmpRef;
            if (Players.TryGetValue(id, out tmpRef))
            {
                if (tmpRef.TryGetTarget(out player))
                    return true;

                Players.TryRemove(id, out tmpRef);
                return false;
            }

            player = null;
            return false;
        }

        internal async Task<Account> CreateAccountAsync()
        {
            int lowId = Interlocked.Increment(ref _seed);
            string token = string.Empty;
            string password = string.Empty;

            for (int i = 0; i < 40; i++)
                token += (char)Resources.Random.Next('A', 'Z');

            for (int i = 0; i < 12; i++)
                password += (char)Resources.Random.Next('A', 'Z');

            Player player = new Player(null, Constants.ServerId, lowId)
            {
                Token = token,
                Password = password
            };

            Home home = new Home(Constants.ServerId, lowId) { LastSave = LevelFile.StartingHome };

            await Mongo.Players.InsertOneAsync(new Players
            {
                HighId = Constants.ServerId,
                LowId = lowId,

                Player = BsonDocument.Parse(JsonConvert.SerializeObject(player, this.Settings)),
                Home = BsonDocument.Parse(JsonConvert.SerializeObject(home, this.Settings))
            });

            Account account = new Account(Constants.ServerId, lowId, player, home);

            this.Add(player);
            this.Add(home);
            this.Add(account);

            Level Level = new Level();
            Level.SetPlayer(player);
            Level.SetHome(home);
            Level.FastForwardTime(0);
            Level.Process();

#if COMMAND_DEBUG
            /* Capture the state of the village. */
            player.Debug.Capture();
#endif

            return account;
        }

        internal async Task<Account> LoadRandomOfflineAccountAsync(bool store = true)
        {
            int serverId = Constants.ServerId;
            int seed = this._seed;
            int rnd = 0;

            Account account = null;

            for (int i = 0; i < 50; i++)
            {
                rnd = Resources.Random.Next(1, seed + 1);

                Account tmp;
                if (!this.TryGetAccount(rnd, out tmp))
                {
                    account = await this.LoadAccountAsync(serverId, rnd, store);

                    if (account != null)
                    {
                        break;
                    }
                }
                else
                {
                    if (tmp.Device == null)
                    {
                        if (tmp.Battle != null)
                        {
                            if (!tmp.Battle.Ended)
                            {
                                continue;
                            }
                        }

                        tmp.Battle = null;
                        account = tmp;
                        break;
                    }
                }
            }

            return account;
        }

        internal Account LoadAccount(int HighID, int LowID, bool Store = true)
        {
            long ID = ((long)HighID << 32) | (uint)LowID;

            Account Account;
            if (!this.TryGetAccount(ID, out Account))
            {
                Players Data = Mongo.Players.Find(T => T.HighId == HighID && T.LowId == LowID).SingleOrDefault();

                if (Data != null)
                {
                    Player Player;
                    if (!this.TryGetPlayer(ID, out Player))
                    {
                        Player = this.LoadPlayerFromSave(Data.Player.ToJson());

                        if (Store)
                        {
                            this.Add(Player);
                        }
                    }

                    Home Home;
                    if (!this.TryGetHome(ID, out Home))
                    {
                        Home = this.LoadHomeFromSave(Data.Home.ToJson());

                        if (Store)
                        {
                            this.Add(Home);
                        }
                    }

                    if (Player == null || Home == null)
                    {
                        Logging.Error(this.GetType(), "Unable to load account id:" + HighID + "-" + LowID + ".");
                        return new Account(-1, -1, null, null);
                    }

                    Account = new Account(HighID, LowID, Player, Home);
                    this.TryAdd(ID, new WeakReference<Account>(Account));
                }
            }

            if (Account != null)
            {
                if (Account.Player.Level == null || Account.Home.Level == null)
                {
                    Level Level = new Level();
                    Level.SetPlayer(Account.Player);
                    Level.SetHome(Account.Home);
                    Level.FastForwardTime(0);
                    Level.Process();
                }
            }

            return Account;
        }

        internal async Task<Account> LoadAccountViaFacebookAsync(string FacebookID, bool Store = true)
        {
            Account Account = null;
            Players Data = await Mongo.Players.Find(T => T.FacebookId == FacebookID).SingleOrDefaultAsync();

            if (Data != null)
            {
                long ID = ((long)Data.HighId << 32) | (uint)Data.LowId;

                if (!this.TryGetAccount(ID, out Account))
                {
                    Player Player;
                    if (!this.TryGetPlayer(ID, out Player))
                    {
                        Player = this.LoadPlayerFromSave(Data.Player.ToJson());

                        if (Store)
                        {
                            this.Add(Player);
                        }
                    }

                    Home Home;
                    if (!this.TryGetHome(ID, out Home))
                    {
                        Home = this.LoadHomeFromSave(Data.Home.ToJson());

                        if (Store)
                        {
                            this.Add(Home);
                        }
                    }

                    if (Player == null || Home == null)
                    {
                        Logging.Error(this.GetType(), $"Unable to load account id:{Data.HighId}-{Data.LowId} and with facebook id {FacebookID}.");
                        return new Account(-1, -1, null, null);
                    }

                    Account = new Account(Data.HighId, Data.LowId, Player, Home);

                    this.Add(Account);
                }

                if (Account != null)
                {
                    if (Account.Player.Level == null && Account.Home.Level == null)
                    {
                        Level Level = new Level();
                        Level.SetPlayer(Account.Player);
                        Level.SetHome(Account.Home);
                        Level.FastForwardTime(0);
                        Level.Process();
                    }
                }
            }

            return Account;
        }

        internal async Task<Account> LoadAccountAsync(int HighID, int LowID, bool Store = true)
        {
            long ID = ((long)HighID << 32) | (uint)LowID;

            Account Account;
            if (!this.TryGetAccount(ID, out Account))
            {
                Players Data = await Mongo.Players.Find(T => T.HighId == HighID && T.LowId == LowID).SingleOrDefaultAsync();

                if (Data != null)
                {
                    Player Player;
                    if (!this.TryGetPlayer(ID, out Player))
                    {
                        Player = this.LoadPlayerFromSave(Data.Player.ToJson());

                        if (Store)
                        {
                            this.Add(Player);
                        }
                    }

                    Home Home;
                    if (!this.TryGetHome(ID, out Home))
                    {
                        Home = this.LoadHomeFromSave(Data.Home.ToJson());

                        if (Store)
                        {
                            this.Add(Home);
                        }
                    }


                    if (Player == null || Home == null)
                    {
                        Logging.Error(this.GetType(), "Unable to load account id:" + HighID + "-" + LowID + ".");
                        return Account;
                    }

                    Account = new Account(HighID, LowID, Player, Home);

                    this.TryAdd(ID, new WeakReference<Account>(Account));
                }
            }

            if (Account != null)
            {
                if (Account.Player.Level == null && Account.Home.Level == null)
                {
                    Level Level = new Level();
                    Level.SetPlayer(Account.Player);
                    Level.SetHome(Account.Home);
                    Level.FastForwardTime(0);
                    Level.Process();

#if COMMAND_DEBUG
                    /* Capture the state of the village. */
                    Account.Player.Debug.Capture();
#endif
                }
            }

            return Account;
        }


        internal async Task<Player> LoadPlayerAsync(int HighID, int LowID, bool Store = true)
        {
            long ID = ((long)HighID << 32) | (uint)LowID;

            Player Player;
            if (!this.TryGetPlayer(ID, out Player))
            {
                Players Data = await Mongo.Players.Find(T => T.HighId == HighID && T.LowId == LowID).SingleOrDefaultAsync();

                if (Data != null)
                {
                    Player = this.LoadPlayerFromSave(Data.Player.ToJson());

                    if (Store)
                    {
                        this.Add(Player);
                    }

                    Home Home;
                    if (!this.TryGetHome(ID, out Home))
                    {
                        Home = this.LoadHomeFromSave(Data.Home.ToJson());

                        if (Store)
                        {
                            this.Add(Home);
                        }
                    }

                    if (Player == null)
                    {
                        Logging.Error(this.GetType(), "Unable to load player id:" + HighID + "-" + LowID + ".");
                        return null;
                    }
                }
            }

            return Player;
        }

        internal async Task<Home> LoadHomeAsync(int HighID, int LowID, bool Store = true)
        {
            long ID = ((long)HighID << 32) | (uint)LowID;

            Home Home;
            if (!this.TryGetHome(ID, out Home))
            {
                Players Data = await Mongo.Players.Find(T => T.HighId == HighID && T.LowId == LowID).SingleOrDefaultAsync();

                if (Data != null)
                {
                    Home = this.LoadHomeFromSave(Data.Home.ToJson());

                    if (Store)
                    {
                        this.Add(Home);
                    }

                    Player Player;
                    if (!this.TryGetPlayer(ID, out Player))
                    {
                        Player = this.LoadPlayerFromSave(Data.Player.ToJson());

                        if (Store)
                        {
                            this.Add(Player);
                        }
                    }

                    if (Home == null)
                    {
                        Logging.Error(this.GetType(), "Unable to load home id:" + HighID + "-" + LowID + ".");
                        return null;
                    }
                }
            }

            return Home;
        }

        internal Player LoadPlayerFromSave(string JSON)
        {
            Player Player = JsonConvert.DeserializeObject<Player>(JSON, this.Settings);
            return Player;
        }

        internal Home LoadHomeFromSave(string JSON)
        {
            //var Home = new Home();
            //JsonConvert.PopulateObject(JSON, Home, this.Settings);
            Home Home = JsonConvert.DeserializeObject<Home>(JSON, this.Settings);
            return Home;
        }

        internal async Task SavePlayer(Player Player)
        {
            await Mongo.Players.UpdateOneAsync(Save => Save.HighId == Player.HighID && Save.LowId == Player.LowID,
                Builders<Players>
                    .Update.Set(Save => Save.Player,
                        BsonDocument.Parse(JsonConvert.SerializeObject(Player, this.Settings))));
        }

        internal async Task SaveHome(Home Home)
        {
            await Mongo.Players.UpdateOneAsync(Save => Save.HighId == Home.HighID && Save.LowId == Home.LowID,
                Builders<Players>.Update.Set(Save => Save.Home,
                    BsonDocument.Parse(JsonConvert.SerializeObject(Home, this.Settings))));
        }

        internal void Saves()
        {
            KeyValuePair<long, WeakReference<Player>>[] players = this.Players.ToArray();
            KeyValuePair<long, WeakReference<Home>>[] homes = this.Homes.ToArray();

            foreach (var kv in players)
            {
                Player player;
                if (kv.Value.TryGetTarget(out player))
                    _savePlayerQueue.Enqueue(player);
                else
                {
                    WeakReference<Player> _;
                    Players.TryRemove(kv.Key, out _);
                }
            }

            foreach (var kv in homes)
            {
                Home home;
                if (kv.Value.TryGetTarget(out home))
                    _saveHomeQueue.Enqueue(home);
                else
                {
                    WeakReference<Home> _;
                    Homes.TryRemove(kv.Key, out _);
                }
            }
        }

        internal Task[] SaveAll()
        {
            KeyValuePair<long, WeakReference<Player>>[] players = this.Players.ToArray();
            KeyValuePair<long, WeakReference<Home>>[] homes = this.Homes.ToArray();
            List<Task> tasks = new List<Task>();

            foreach (var kv in players)
            {
                Player player;
                if (kv.Value.TryGetTarget(out player))
                {
                    tasks.Add(SavePlayer(player));
                }
                else
                {
                    WeakReference<Player> _;
                    Players.TryRemove(kv.Key, out _);
                }
            }

            foreach (var kv in homes)
            {
                Home home;
                if (kv.Value.TryGetTarget(out home))
                {
                    tasks.Add(SaveHome(home));
                }
                else
                {
                    WeakReference<Home> _;
                    Homes.TryRemove(kv.Key, out _);
                }
            }

            return tasks.ToArray();
        }

        internal Player[] GetAllPlayers()
        {
            KeyValuePair<long, WeakReference<Player>>[] playerRefsKv = this.Players.ToArray();
            List<Player> players = new List<Player>(playerRefsKv.Length);

            for (int i = 0; i < playerRefsKv.Length; i++)
            {
                Player player;
                KeyValuePair<long, WeakReference<Player>> playerRefKv = playerRefsKv[i];

                if (playerRefKv.Value.TryGetTarget(out player))
                    players.Add(player);
                else
                {
                    WeakReference<Player> _;
                    Players.TryRemove(playerRefKv.Key, out _);
                }
            }

            return players.ToArray();
        }

        private void SavePlayerTask()
        {
            while (true)
            {
                Player Player;
                while (_savePlayerQueue.TryDequeue(out Player))
                {
                    try
                    {
                        Mongo.Players.UpdateOne(Save => Save.HighId == Player.HighID && Save.LowId == Player.LowID,
                            Builders<Players>
                                .Update.Set(Save => Save.Player,
                                    BsonDocument.Parse(JsonConvert.SerializeObject(Player, Resources.Accounts.Settings))));
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(typeof(Accounts), "Failed to save player: " + ex);
                    }
                }

                /* Look for dead references. */
                foreach (var kv in this)
                {
                    Account _;
                    if (!kv.Value.TryGetTarget(out _))
                    {
                        WeakReference<Account> __;
                        TryRemove(kv.Key, out __);
                    }
                }

                Thread.Sleep(200);
            }
        }

        private static void SaveHomeTask()
        {
            while (true)
            {
                Home Home;
                while (_saveHomeQueue.TryDequeue(out Home))
                {
                    try
                    {
                        Mongo.Players.UpdateOne(Save => Save.HighId == Home.HighID && Save.LowId == Home.LowID,
                            Builders<Players>
                                .Update.Set(Save => Save.Home,
                                    BsonDocument.Parse(JsonConvert.SerializeObject(Home, Resources.Accounts.Settings))));
                    }
                    catch (Exception ex)
                    {
                        Logging.Error(typeof(Accounts), "Failed to save home: " + ex);
                    }
                }

                Thread.Sleep(5);
            }
        }
    }
}