namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using Newtonsoft.Json.Linq;

    internal class HeroBaseComponent : Component
    {
        internal HeroData HeroData;

        internal Timer UpgradeTimer;

        public HeroBaseComponent(GameObject GameObject, HeroData HeroData) : base(GameObject)
        {
            this.HeroData = HeroData;
        }

        internal override int Type
        {
            get
            {
                return 10;
            }
        }

        internal int RemainingUpgradeTime
        {
            get
            {
                return this.UpgradeTimer?.GetRemainingSeconds(this.Parent.Level.Player.LastTick) ?? 0;
            }
        }

        internal int UpgradeLevel
        {
            get
            {
                return this.Parent.Level.Player.GetHeroUpgradeLevel(this.HeroData);
            }
        }

        internal int VillageType
        {
            get
            {
                return this.HeroData.VillageType;
            }
        }

        internal bool Upgrading
        {
            get
            {
                return this.UpgradeTimer != null;
            }
        }

        internal bool UpgradeAvailable
        {
            get
            {
                if (!this.Upgrading)
                {
                    Level Level = this.Parent.Level;
                    HeroData Data = this.HeroData;

                    if (Data.MaxLevel > this.UpgradeLevel)
                    {
                        return (this.VillageType == 1 ? Level.GameObjectManager.TownHall2.GetUpgradeLevel() + 1 : Level.GameObjectManager.TownHall.GetUpgradeLevel() + 1) >= Data.RequiredTownHallLevel[this.UpgradeLevel + 1];
                    }
                }

                return false;
            }
        }

        internal void StartUpgrade()
        {
            int Time = this.HeroData.GetUpgradeTime(this.UpgradeLevel);

            if (!this.Upgrading)
            {
                if (this.VillageType == 0)
                {
                    this.Parent.Level.WorkerManager.AllocateWorker(this.Parent);
                }
                else
                {
                    this.Parent.Level.WorkerManagerV2.AllocateWorker(this.Parent);
                }

                this.Parent.Level.Player.HeroStates.Set(this.HeroData, 1);

                if (Time <= 0)
                {
                    this.FinishUpgrading();
                }
                else
                {
                    this.UpgradeTimer = new Timer();
                    this.UpgradeTimer.StartTimer(this.Parent.Level.Player.LastTick, Time);
                }
            }
        }


        internal void SpeedUpUpgrade()
        {
            if (this.Parent.Level.Player != null)
            {
                if (this.Upgrading)
                {
                    int Cost = GamePlayUtil.GetSpeedUpCost(this.RemainingUpgradeTime, this.VillageType, 100);

                    if (this.Parent.Level.Player.HasEnoughDiamonds(Cost))
                    {
                        this.Parent.Level.Player.UseDiamonds(Cost);
                        this.FinishUpgrading();
                    }
                }
            }
        }

        internal void CancelUpgrade()
        {
            Player Player = this.Parent.Level.Player;

            if (Player != null)
            {
                if (this.Upgrading)
                {
                    int CurrentUpgrade = Player.GetHeroUpgradeLevel(this.HeroData);

                    int resourceCount = (int) ((this.HeroData.UpgradeCost[CurrentUpgrade] * Globals.HeroUpgradeCancelMultiplier * (long) 1374389535) >> 32);
                    resourceCount = Math.Max((resourceCount >> 5) + (resourceCount >> 31), 0);

                    Player.Resources.Plus(this.HeroData.GlobalId, resourceCount);

                    if (this.VillageType == 0)
                    {
                        this.Parent.Level.WorkerManager.DeallocateWorker(this.Parent);
                    }
                    else
                    {
                        this.Parent.Level.WorkerManagerV2.DeallocateWorker(this.Parent);
                    }

                    this.UpgradeTimer = null;
                }
                else
                {
                    Logging.Error(this.GetType(), "Tried to cancel hero upgrade but UpgradeOnGoing returned false");
                }
            }
        }

        internal void FinishUpgrading()
        {
            if (this.UpgradeLevel + 1 > this.HeroData.MaxLevel)
            {
                Logging.Error(this.GetType(), "Unable to upgrade the hero because the level is out of range! - " + this.HeroData.Name + ".");
                this.Parent.Level.Player.HeroUpgrades.Set(this.HeroData, this.HeroData.MaxLevel);
            }
            else
            {
                this.Parent.Level.Player.IncreaseHeroUpgradeLevel(this.HeroData);
            }

            if (this.VillageType == 0)
            {
                this.Parent.Level.WorkerManager.DeallocateWorker(this.Parent);
            }
            else
            {
                this.Parent.Level.WorkerManagerV2.DeallocateWorker(this.Parent);
            }

            this.Parent.Level.Player.HeroStates.Set(this.HeroData, 3);

            this.UpgradeTimer = null;
        }

        internal override void FastForwardTime(int Seconds)
        {
            if (this.Upgrading)
            {
                this.UpgradeTimer.FastForward(Seconds);
            }

            base.FastForwardTime(Seconds);
        }

        internal override void Tick()
        {
            if (this.Upgrading)
            {
                if (this.UpgradeTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0)
                {
                    this.FinishUpgrading();
                }
            }

            base.Tick();
        }

        internal override void Load(JToken Json)
        {
            JToken HeroUpgrade = Json["hero_upg"];

            if (HeroUpgrade != null)
            {
                int UpgradeTime;
                int UpgradeTimeEnd;
                if (JsonHelper.GetJsonNumber(HeroUpgrade, "t", out UpgradeTime) && JsonHelper.GetJsonNumber(HeroUpgrade, "t_end", out UpgradeTimeEnd))
                {
                    if (UpgradeTime > -1)
                    {
                        int startTime = (int) TimeUtils.ToUnixTimestamp(this.Parent.Level.Player.LastTick);

                        int duration = UpgradeTimeEnd - startTime;
                        if (duration < 0)
                        {
                            duration = 0;
                        }

                        //duration = Math.Min(duration, this.HeroData.GetUpgradeTime(this.UpgradeLevel + 1));

                        this.UpgradeTimer = new Timer();
                        this.UpgradeTimer.StartTimer(this.Parent.Level.Player.LastTick, duration);

                        if (this.VillageType == 0)
                        {
                            this.Parent.Level.WorkerManager.AllocateWorker(this.Parent);
                        }
                        else
                        {
                            this.Parent.Level.WorkerManagerV2.AllocateWorker(this.Parent);
                        }
                    }
                }

                int Level;
                if (JsonHelper.GetJsonNumber(HeroUpgrade, "level", out Level))
                {
                    if (Level > this.UpgradeLevel + 1)
                    {
                        Logging.Error(this.GetType(), "'Level' seems to be bigger than 'this.UpgradeLevel + 1'");
                    }
                }
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            if (this.Upgrading)
            {
                JObject HeroUpgrade = new JObject
                {
                    {"t", this.UpgradeTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick)},
                    {"t_end", this.UpgradeTimer.EndTime},
                    {"level", this.UpgradeLevel + 1}
                };

                Json.Add("hero_upg", HeroUpgrade);
            }
        }
    }
}