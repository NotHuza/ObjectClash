namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using Newtonsoft.Json.Linq;

    internal class VillageObject : GameObject
    {
        internal Timer ConstructionTimer;
        private int UpgradeLevel;

        public VillageObject(Data Data, Level Level) : base(Data, Level)
        {
        }

        internal VillageObjectData VillageObjectData
        {
            get
            {
                return (VillageObjectData) this.Data;
            }
        }

        internal override int Type
        {
            get
            {
                return 8;
            }
        }

        internal override int VillageType
        {
            get
            {
                return this.VillageObjectData.VillageType;
            }
        }

        internal int RemainingConstructionTime
        {
            get
            {
                return this.ConstructionTimer?.GetRemainingSeconds(this.Level.Player.LastTick) ?? 0;
            }
        }

        internal bool Constructing
        {
            get
            {
                return this.ConstructionTimer != null;
            }
        }

        internal override int HeightInTiles
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
                return 0;
            }
        }

        internal int GetUpgradeLevel()
        {
            return this.UpgradeLevel;
        }

        internal void FinishConstruction()
        {
            VillageObjectData Data = this.VillageObjectData;

            if (this.UpgradeLevel + 1 > Data.MaxLevel)
            {
                Logging.Error(this.GetType(), "Unable to upgrade the building because the level is out of range! - " + Data.Name + ".");
                this.SetUpgradeLevel(Data.MaxLevel);
            }
            else
            {
                this.SetUpgradeLevel(this.UpgradeLevel + 1);
            }

            if (this.VillageType == 0)
            {
                this.Level.WorkerManager.DeallocateWorker(this);
            }
            else
            {
                this.Level.WorkerManagerV2.DeallocateWorker(this);
            }

            this.Level.Player.AddExperience(GamePlayUtil.TimeToXp(Data.GetBuildTime(this.UpgradeLevel)));

            this.ConstructionTimer = null;
        }

        internal void StartUpgrade()
        {
            int Time = this.VillageObjectData.GetBuildTime(this.UpgradeLevel + 1);

            if (!this.Constructing)
            {
                if (this.VillageType == 0)
                {
                    this.Level.WorkerManager.AllocateWorker(this);
                }
                else
                {
                    this.Level.WorkerManagerV2.AllocateWorker(this);
                }

                if (Time <= 0)
                {
                    this.FinishConstruction();
                }
                else
                {
                    this.ConstructionTimer = new Timer();
                    this.ConstructionTimer.StartTimer(this.Level.Player.LastTick, Time);
                }
            }
        }

        internal void SpeedUpConstruction()
        {
            if (this.Level.Player != null)
            {
                if (this.Constructing)
                {
                    int Cost = GamePlayUtil.GetSpeedUpCost(this.RemainingConstructionTime, this.VillageObjectData.VillageType, 100);

                    if (this.Level.Player.HasEnoughDiamonds(Cost))
                    {
                        this.Level.Player.UseDiamonds(Cost);
                        this.FinishConstruction();
                    }
                }
            }
        }

        internal void SetUpgradeLevel(int UpgradeLevel)
        {
            this.UpgradeLevel = UpgradeLevel;
        }

        internal override void FastForwardTime(int Seconds)
        {
            if (this.Constructing)
            {
                this.ConstructionTimer.FastForward(Seconds);
            }

            base.FastForwardTime(Seconds);
        }


        internal override void Tick()
        {
            if (this.Constructing)
            {
                if (this.ConstructionTimer.GetRemainingSeconds(this.Level.Player.LastTick) <= 0)
                {
                    this.FinishConstruction();
                }
            }

            base.Tick();
        }

        internal override void Load(JToken Json)
        {
            int ConstructionTime;
            int ConstructionTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "const_t", out ConstructionTime) && JsonHelper.GetJsonNumber(Json, "const_t_end", out ConstructionTimeEnd))
            {
                if (ConstructionTime > -1)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Level.Player.LastTick);
                    int duration = ConstructionTimeEnd - startTime;

                    //ConstructionTime = Math.Min(ConstructionTime, Data.GetBuildTime(this.UpgradeLevel + 1));

                    this.ConstructionTimer = new Timer();
                    this.ConstructionTimer.StartTimer(this.Level.Player.LastTick, duration);
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

            int Level;
            if (JsonHelper.GetJsonNumber(Json, "lvl", out Level))
            {
                if (Level < -1)
                {
                    Logging.Error(this.GetType(),
                        "An error has been throwed when the loading of village object - Load an illegal upgrade level. Level : " +
                        Level);
                    this.SetUpgradeLevel(0);
                }
                else
                {
                    this.SetUpgradeLevel(Level);
                }
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            Json.Add("lvl", this.UpgradeLevel);

            if (this.ConstructionTimer != null)
            {
                Json.Add("const_t", this.ConstructionTimer.GetRemainingSeconds(this.Level.Player.LastTick));
                Json.Add("const_t_end", this.ConstructionTimer.EndTime);
            }

            base.Save(Json);
        }
    }
}