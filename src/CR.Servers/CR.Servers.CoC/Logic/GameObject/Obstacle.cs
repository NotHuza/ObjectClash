namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using Newtonsoft.Json.Linq;

    internal class Obstacle : GameObject
    {
        internal Timer ClearTimer;
        internal bool Destructed;

        internal int[] GemDrops =
        {
            3, 0, 1, 2, 0, 1, 1, 0, 0, 3,
            1, 0, 2, 2, 0, 0, 3, 0, 1, 0
        };

        public Obstacle(Data Data, Level Level) : base(Data, Level)
        {
        }

        internal ObstacleData ObstacleData
        {
            get
            {
                return (ObstacleData) this.Data;
            }
        }

        internal override int Type
        {
            get
            {
                return 3;
            }
        }

        internal override int Checksum
        {
            get
            {
                return 0;
            }
        }

        internal override int WidthInTiles
        {
            get
            {
                return this.ObstacleData.Width;
            }
        }

        internal override int HeightInTiles
        {
            get
            {
                return this.ObstacleData.Height;
            }
        }

        internal override int VillageType
        {
            get
            {
                return this.ObstacleData.VillageType;
            }
        }

        internal int RemainingClearingTime
        {
            get
            {
                return this.ClearTimer?.GetRemainingSeconds(this.Level.Player.LastTick) ?? 0;
            }
        }

        internal bool ClearingOnGoing
        {
            get
            {
                return this.ClearTimer != null;
            }
        }

        internal bool CanClearing()
        {
            if (!this.ClearingOnGoing)
            {
                if (!this.Destructed)
                {
                    return true;
                }
            }

            return false;
        }

        internal void SpeedUpClearing()
        {
            if (this.Level.Player != null)
            {
                if (this.ClearingOnGoing)
                {
                    int Cost = GamePlayUtil.GetSpeedUpCost(this.RemainingClearingTime, this.ObstacleData.VillageType, 100);

                    if (this.Level.Player.HasEnoughDiamonds(Cost))
                    {
                        this.Level.Player.UseDiamonds(Cost);
                        this.ClearingFinished();
                    }
                }
            }
        }

        internal void ClearingFinished()
        {
            Player Player = this.Level.Player;

            if (this.VillageType == 0)
            {
                Player.AddDiamonds(this.GemDrops[this.Level.GameObjectManager.ObstacleClearCounter++]);

                if (this.Level.GameObjectManager.ObstacleClearCounter >= this.GemDrops.Length)
                {
                    this.Level.GameObjectManager.ObstacleClearCounter = 0;
                }

                this.Level.WorkerManager.DeallocateWorker(this);
            }
            else
            {
                if (!this.ObstacleData.TallGrass)
                {
                    Player.AddDiamonds(this.GemDrops[this.Level.GameObjectManager.ObstacleClearCounterV2++]);

                    if (this.Level.GameObjectManager.ObstacleClearCounterV2 >= this.GemDrops.Length)
                    {
                        this.Level.GameObjectManager.ObstacleClearCounterV2 = 0;
                    }

                    this.Level.WorkerManagerV2.DeallocateWorker(this);
                }
            }

            // LogicAchievementManager::obstacleCleared();

            Player.AddExperience(GamePlayUtil.TimeToXp(this.ObstacleData.ClearTimeSeconds));
            Player.ObstacleCleaned++;

            this.ClearTimer = null;
            this.Destructed = true;
            this.Level.GameObjectManager.RemoveGameObject(this, this.VillageType);
        }

        internal void StartClearing()
        {
            ObstacleData Data = this.ObstacleData;

            if (!this.ClearingOnGoing)
            {
                if (this.VillageType == 0)
                {
                    this.Level.WorkerManager.AllocateWorker(this);
                }
                else
                {
                    if (!this.ObstacleData.TallGrass)
                    {
                        this.Level.WorkerManagerV2.AllocateWorker(this);
                    }
                }

                if (Data.ClearTimeSeconds <= 0)
                {
                    this.ClearingFinished();
                }
                else
                {
                    this.ClearTimer = new Timer();
                    this.ClearTimer.StartTimer(this.Level.Player.LastTick, Data.ClearTimeSeconds);
                }
            }
        }

        internal void CancelClearing()
        {
            if (this.ClearingOnGoing)
            {
                this.Level.Player.Resources.Plus(this.ObstacleData.ClearResourceData.GlobalId, this.ObstacleData.ClearCost);

                if (this.VillageType == 0)
                {
                    this.Level.WorkerManager.DeallocateWorker(this);
                }
                else
                {
                    this.Level.WorkerManagerV2.DeallocateWorker(this);
                }

                this.ClearTimer = null;
            }
        }

        internal override void FastForwardTime(int Seconds)
        {
            if (this.ClearingOnGoing)
            {
                this.ClearTimer.FastForward(Seconds);
            }

            base.FastForwardTime(Seconds);
        }

        internal override void Tick()
        {
            if (this.ClearingOnGoing)
            {
                if (this.ClearTimer.GetRemainingSeconds(this.Level.Player.LastTick) <= 0)
                {
                    this.ClearingFinished();
                }
            }

            base.Tick();
        }

        internal override void Load(JToken Json)
        {
            int ClearTime;
            int ClearTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "clear_t", out ClearTime) && JsonHelper.GetJsonNumber(Json, "clear_t_end", out ClearTimeEnd))
            {
                if (ClearTime > -1)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Level.Player.LastTick);
                    int duration = ClearTimeEnd - startTime;
                    if (duration < 0)
                    {
                        duration = 0;
                    }

                    this.ClearTimer = new Timer();
                    this.ClearTimer.StartTimer(this.Level.Player.LastTick, duration);

                    if (this.VillageType == 0)
                    {
                        this.Level.WorkerManager.AllocateWorker(this);
                    }
                    else
                    {
                        this.Level.WorkerManagerV2.AllocateWorker(this);
                    }
                }
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            if (this.ClearingOnGoing)
            {
                Json.Add("clear_t", this.ClearTimer.GetRemainingSeconds(this.Level.Player.LastTick));
                Json.Add("clear_t_end", this.ClearTimer.EndTime);
            }

            base.Save(Json);
        }
    }
}