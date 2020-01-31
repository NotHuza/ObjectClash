namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using Newtonsoft.Json.Linq;

    internal class Building : GameObject
    {
        internal bool BoostPause;

        internal Timer BoostTimer;
        internal Timer ConstructionTimer;

        internal bool Gearing;

        internal bool Locked;
        private int UpgradeLevel;

        public Building(Data Data, Level Level) : base(Data, Level)
        {
            BuildingData BuildingData = this.BuildingData;

            if (BuildingData.IsTrainingHousing)
            {
                this.AddComponent(new UnitStorageComponent(this));
            }

            if (BuildingData.IsDefense || BuildingData.IsWallStraight)
            {
                this.AddComponent(new CombatComponent(this));
            }

            if (!string.IsNullOrEmpty(BuildingData.ProducesResource))
            {
                this.AddComponent(new ResourceProductionComponent(this));
            }

            if (BuildingData.CanStoreResources)
            {
                this.AddComponent(new ResourceStorageComponent(this)
                {
                    MaxArray = BuildingData.GetResourceMaxArray(0)
                });
            }

            if (BuildingData.UnitProduction[0] > 0)
            {
                this.AddComponent(new UnitProductionComponent(this));
            }

            if (BuildingData.Bunker)
            {
                this.AddComponent(new BunkerComponent(this));
                //this.AddComponent(new UnitStorageComponent(this));
            }

            if (BuildingData.UpgradesUnits)
            {
                this.AddComponent(new UnitUpgradeComponent(this));
            }

            if (BuildingData.IsHeroBarrack)
            {
                HeroData hd = CSV.Tables.Get(Gamefile.Heroes).GetData(BuildingData.HeroType) as HeroData;
                this.AddComponent(new HeroBaseComponent(this, hd));
            }

            if (BuildingData.Village2Housing > 0)
            {
                this.AddComponent(new UnitStorageV2Component(this));
            }
        }

        internal BuildingData BuildingData
        {
            get
            {
                return (BuildingData)this.Data;
            }
        }


        internal override int HeightInTiles
        {
            get
            {
                return this.BuildingData.Height;
            }
        }

        internal override int WidthInTiles
        {
            get
            {
                return this.BuildingData.Width;
            }
        }

        internal override int Type
        {
            get
            {
                return 0;
            }
        }

        internal override int VillageType
        {
            get
            {
                return this.BuildingData.VillageType;
            }
        }

        internal override int Checksum
        {
            get
            {
                int Checksum = 0;

                Checksum += base.Checksum;

                ResourceProductionComponent ResourceProductionComponent = this.ResourceProductionComponent;

                if (ResourceProductionComponent != null)
                {
                    Checksum += ResourceProductionComponent.Checksum;
                }

                ResourceStorageComponent ResourceStorageComponent = this.ResourceStorageComponent;

                if (ResourceStorageComponent != null)
                {
                    Checksum += ResourceStorageComponent.Checksum;
                }

                return Checksum;
            }
        }

        internal int RemainingConstructionTime
        {
            get
            {
                return this.ConstructionTimer?.GetRemainingSeconds(this.Level.Player.LastTick) ?? 0;
            }
        }

        internal bool Boosted
        {
            get
            {
                return this.BoostTimer != null;
            }
        }

        internal bool Constructing
        {
            get
            {
                return this.ConstructionTimer != null;
            }
        }

        internal bool UpgradeAvailable
        {
            get
            {
                if (!this.Constructing)
                {
                    BuildingData Data = this.BuildingData;

                    if (Data.MaxLevel > this.UpgradeLevel)
                    {
                        if (this.VillageType == 1) //Shall we check item village type?
                        {
                            return this.Level.GameObjectManager.TownHall2.GetUpgradeLevel() + 1 >=
                                   Data.TownHallLevel2[this.UpgradeLevel + 1];
                        }

                        return this.Level.GameObjectManager.TownHall.GetUpgradeLevel() + 1 >=
                               Data.TownHallLevel[this.UpgradeLevel + 1];
                    }
                }

                return false;
            }
        }

        internal UnitStorageComponent UnitStorageComponent
        {
            get
            {
                Component Component;
                return this.TryGetComponent(0, out Component) ? (UnitStorageComponent)Component : null;
            }
        }

        internal CombatComponent CombatComponent
        {
            get
            {
                Component Component;
                return this.TryGetComponent(1, out Component) ? (CombatComponent)Component : null;
            }
        }

        internal ResourceProductionComponent ResourceProductionComponent
        {
            get
            {
                Component Component;
                return this.TryGetComponent(5, out Component) ? (ResourceProductionComponent)Component : null;
            }
        }

        internal ResourceStorageComponent ResourceStorageComponent
        {
            get
            {
                Component Component;
                return this.TryGetComponent(6, out Component) ? (ResourceStorageComponent)Component : null;
            }
        }

        internal BunkerComponent BunkerComponent
        {
            get
            {
                Component Component;
                return this.TryGetComponent(7, out Component) ? (BunkerComponent)Component : null;
            }
        }

        internal UnitUpgradeComponent UnitUpgradeComponent
        {
            get
            {
                Component Component;
                return this.TryGetComponent(9, out Component) ? (UnitUpgradeComponent)Component : null;
            }
        }

        internal HeroBaseComponent HeroBaseComponent
        {
            get
            {
                Component Component;
                return this.TryGetComponent(10, out Component) ? (HeroBaseComponent)Component : null;
            }
        }

        internal UnitStorageV2Component UnitStorageV2Component
        {
            get
            {
                Component Component;
                return this.TryGetComponent(11, out Component) ? (UnitStorageV2Component)Component : null;
            }
        }


        internal int GetUpgradeLevel()
        {
            return this.UpgradeLevel;
        }

        internal void FinishConstruction()
        {
            BuildingData Data = this.BuildingData;

            if (this.Gearing)
            {
                this.Gearing = false;

                this.CombatComponent.GearUp = 1;

                if (this.CombatComponent.AltAttackMode)
                {
                    this.CombatComponent.AttackMode = true;
                    this.CombatComponent.AttackModeDraft = true;
                }

                this.Level.WorkerManagerV2.DeallocateWorker(this);
            }
            else
            {
                if (this.UpgradeLevel + 1 > Data.MaxLevel)
                {
                    Logging.Error(this.GetType(), "Unable to upgrade the building because the level is out of range! - " + Data.Name + ".");
                    this.SetUpgradeLevel(Data.MaxLevel);
                }
                else
                {
                    this.SetUpgradeLevel(this.UpgradeLevel + 1);
                }

                if (this.Locked)
                {
                    this.Locked = false;
                    if (this.VillageType == 1)
                    {
                        this.Level.WorkerManagerV2.DeallocateWorker(this);
                    }
                }
                else
                {
                    if (this.VillageType == 0)
                    {
                        this.Level.WorkerManager.DeallocateWorker(this);
                    }
                    else
                    {
                        this.Level.WorkerManagerV2.DeallocateWorker(this);
                    }
                }
            }

            if (!Data.IsTroopHousingV2)
            {
                this.Level.Player.AddExperience(GamePlayUtil.TimeToXp(Data.GetBuildTime(this.UpgradeLevel)));
            }
            else
            {
                int troopHousing = this.Level.GameObjectManager.Filter.GetGameObjectCount(this.BuildingData);

                if (troopHousing >= 0)
                {
                    int Time;
                    switch (troopHousing)
                    {
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            Time = Globals.TroopHousingV2BuildTimeSeconds[troopHousing - 1];
                            break;

                        default:
                            Time = Globals.TroopHousingV2BuildTimeSeconds[4];
                            break;
                    }

                    this.Level.Player.AddExperience(GamePlayUtil.TimeToXp(Time));
                }
                else
                {
                    Logging.Error(this.GetType(), "TroopHousingV2 count is below zero when trying to get build time");
                }
            }

            if (this.HeroBaseComponent != null)
            {
                var data = CSV.Tables.Get(Gamefile.Heroes).GetData(this.BuildingData.HeroType);
                if (data is HeroData)
                {
                    HeroData HeroData = (HeroData)data;

                    this.Level.Player.HeroUpgrades.Set(HeroData, 0);
                    this.Level.Player.HeroStates.Set(HeroData, 3);
                    if (HeroData.HasAltMode)
                    {
                        this.Level.Player.HeroModes.Set(HeroData, 0);
                    }
                }
            }

            this.ConstructionTimer = null;
        }

        internal void CancelConstruction()
        {
            if (this.Constructing)
            {
                this.SetUpgradeLevel(this.UpgradeLevel);
                //Alt resource not supported yet

                int resourceCount = (int)((this.BuildingData.BuildCost[this.UpgradeLevel + 1] * Globals.BuildCancelMultiplier * (long)1374389535) >> 32);
                resourceCount = Math.Max((resourceCount >> 5) + (resourceCount >> 31), 0);

                this.Level.Player.Resources.Plus(this.BuildingData.GlobalId, resourceCount);
                if (this.VillageType == 0)
                {
                    this.Level.WorkerManager.DeallocateWorker(this);
                }
                else
                {
                    this.Level.WorkerManagerV2.DeallocateWorker(this);
                }

                this.ConstructionTimer = null;

                if (this.UpgradeLevel == -1)
                {
                    this.Level.GameObjectManager.RemoveGameObject(this, this.VillageType); //Should never happend since supercell disable this
                }
            }
        }


        internal void StartUpgrade()
        {
            int Time = this.BuildingData.GetBuildTime(this.UpgradeLevel + 1);

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
                    this.ResourceProductionComponent?.CollectResources();

                    this.ConstructionTimer = new Timer();
                    this.ConstructionTimer.StartTimer(this.Level.Player.LastTick, Time);
                }
            }
        }

        internal void StartGearing()
        {
            int Time = this.BuildingData.GetGearUpTime(this.UpgradeLevel);
            if (this.CombatComponent.GearUp != 1)
            {
                if (!this.Constructing)
                {
                    this.Level.WorkerManagerV2.AllocateWorker(this);
                    this.Gearing = true;

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
                else
                {
                    Logging.Error(this.GetType(), "Unable to gear up the building because the buidling is already under construction! - " + this.Data.Name + ".");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to gear up the building because the buidling is already geared up! - " + this.Data.Name + ".");
            }
        }

        internal void SpeedUpConstruction()
        {
            if (this.Level.Player != null)
            {
                if (this.Constructing)
                {
                    int Cost = GamePlayUtil.GetSpeedUpCost(this.RemainingConstructionTime, this.VillageType, 100);

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

            if (this.Level.Player != null)
            {
                if (BuildingData.IsAllianceCastle)
                {
                    if (!this.Locked)
                    {
                        this.Level.Player.SetAllianceCastleLevel(UpgradeLevel);
                    }
                }
                else if (BuildingData.IsTownHall)
                {
                    this.Level.Player.TownHallLevel = UpgradeLevel;
                }
                else if (BuildingData.IsTownHall2)
                {
                    this.Level.Player.TownHallLevel2 = UpgradeLevel;
                }
                else if (BuildingData.IsBarrack2)
                {
                    this.Level.Player.Variables.Village2BarrackLevel = UpgradeLevel;
                }
            }

            ResourceProductionComponent ResourceProductionComponent = this.ResourceProductionComponent;

            ResourceProductionComponent?.SetProduction();

            ResourceStorageComponent ResourceStorageComponent = this.ResourceStorageComponent;

            if (ResourceStorageComponent != null)
            {
                if (UpgradeLevel > -1)
                {
                    ResourceStorageComponent.SetMaxArray(this.BuildingData.GetResourceMaxArray(UpgradeLevel));
                    if (!this.Level.AI)
                    {
                        this.Level.ComponentManager.RefreshResourceCaps();
                    }
                }
            }

            UnitStorageComponent UnitStorageComponent = this.UnitStorageComponent;

            if (UnitStorageComponent != null)
            {
                if (UpgradeLevel > -1)
                {
                    UnitStorageComponent.SetStorage();
                }
            }
        }

        internal override void FastForwardTime(int Seconds)
        {
            if (this.Constructing)
            {
                this.ConstructionTimer.FastForward(Seconds);
            }

            if (this.Boosted)
            {
                this.BoostTimer.FastForward(Seconds);
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

            if (this.Boosted)
            {
                if (this.BoostTimer.GetRemainingSeconds(this.Level.Player.LastTick) <= 0)
                {
                    this.BoostTimer = null;
                }
            }

            base.Tick();
        }

        internal override void Load(JToken Json)
        {
            BuildingData Data = this.BuildingData;

            if (this.VillageType == 0)
            {
                this.Level.WorkerManager.DeallocateWorker(this);
            }
            else
            {
                this.Level.WorkerManagerV2.DeallocateWorker(this);
            }
            
            if (Data.Locked)
            {
                JsonHelper.GetJsonBoolean(Json, "locked", out this.Locked);
            }

            int ConstructionTime;
            int ConstructionTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "const_t", out ConstructionTime) && JsonHelper.GetJsonNumber(Json, "const_t_end", out ConstructionTimeEnd))
            {
                if (ConstructionTime > -1)
                {
                    int startTime = (int)TimeUtils.ToUnixTimestamp(this.Level.Player.LastTick);
                    int duration = ConstructionTimeEnd - startTime;
                    if (duration < 0)
                    {
                        duration = 0;
                    }
                    //ConstructionTime = Math.Min(ConstructionTime, Data.GetBuildTime(this.UpgradeLevel + 1));

                    this.ConstructionTimer = new Timer();
                    this.ConstructionTimer.StartTimer(this.Level.Player.LastTick, duration);


                    bool Gearing;
                    if (JsonHelper.GetJsonBoolean(Json, "gearing", out Gearing))
                    {
                        this.Gearing = Gearing;
                        this.Level.WorkerManagerV2.AllocateWorker(this);
                    }
                    else
                    {
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
            }

            int BoostTime;
            int BoostTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "boost_t", out BoostTime) && JsonHelper.GetJsonNumber(Json, "boost_t_end", out BoostTimeEnd))
            {
                if (BoostTime > -1)
                {
                    int startTime = (int)TimeUtils.ToUnixTimestamp(this.Level.Player.LastTick);
                    int duration = BoostTimeEnd - startTime;
                    if (duration < 0)
                    {
                        duration = 0;
                    }

                    this.BoostTimer = new Timer();
                    this.BoostTimer.StartTimer(this.Level.Player.LastTick, duration);
                }
            }

            JsonHelper.GetJsonBoolean(Json, "boost_pause", out this.BoostPause);

            int Level;
            if (JsonHelper.GetJsonNumber(Json, "lvl", out Level))
            {
                if (Level < -1)
                {
                    if (this.VillageType != 1)
                    {
                        if (!Data.Locked)
                        {
                            Logging.Error(this.GetType(), "An error has been throwed when the loading of building - Load an illegal upgrade level. Level : " + Level);
                            this.SetUpgradeLevel(0);
                        }
                    }
                }
                else if (Level > Data.MaxLevel)
                {
                    Logging.Error(this.GetType(), $"An error has been throwed when the loading of building - Loaded upgrade level {Level + 1} is over max! (max = {Data.MaxLevel + 1}) id {this.Id} data id {Data.GlobalId}");
                    this.SetUpgradeLevel(Data.MaxLevel);
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

            if (this.Locked)
            {
                Json.Add("locked", this.Locked);
            }

            if (this.Gearing)
            {
                Json.Add("gearing", this.Gearing);
            }

            if (this.ConstructionTimer != null)
            {
                Json.Add("const_t", this.ConstructionTimer.GetRemainingSeconds(this.Level.Player.LastTick));
                Json.Add("const_t_end", this.ConstructionTimer.EndTime);
            }

            if (this.BoostTimer != null)
            {
                Json.Add("boost_t", this.BoostTimer.GetRemainingSeconds(this.Level.Player.LastTick));
                Json.Add("boost_t_end", this.BoostTimer.EndTime);
            }


            if (this.BoostPause)
            {
                Json.Add("boost_pause", this.BoostPause);
            }

            base.Save(Json);
        }
    }
}