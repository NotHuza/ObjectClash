namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.Titan;

    internal class WorkerManagerV2
    {
        internal LogicArrayList<GameObject> GameObjects;
        internal int WorkerCount;

        public WorkerManagerV2()
        {
            this.GameObjects = new LogicArrayList<GameObject>();
            this.WorkerCount = 1;
        }

        internal int FreeWorkers
        {
            get
            {
                return this.WorkerCount - this.GameObjects.Count;
            }
        }

        internal void AllocateWorker(GameObject GameObject)
        {
            int index = this.GameObjects.IndexOf(GameObject);

            if (index != -1)
            {
                Logging.Error(this.GetType(), "WorkerManagerV2::allocateWorker called twice for same target!");
                return;
            }

            this.GameObjects.Add(GameObject);
        }

        internal void DeallocateWorker(GameObject GameObject)
        {
            int index = this.GameObjects.IndexOf(GameObject);

            if (index != -1)
            {
                this.GameObjects.Remove(index);
            }
        }

        internal GameObject GetShortestTaskGO()
        {
            if (this.GameObjects.Count > 0)
            {
                Task Task = new Task();

                for (int i = 0; i < GameObjects.Count; i++)
                {
                    GameObject Construction = GameObjects[i];
                    int RemainingTime = -1;

                    if (Construction is Building)
                    {
                        Building Building = (Building)Construction;
                        HeroBaseComponent HeroBaseComponent = Building.HeroBaseComponent;
                        if (HeroBaseComponent != null)
                        {
                            if (HeroBaseComponent.Upgrading)
                            {
                                RemainingTime = HeroBaseComponent.RemainingUpgradeTime;
                            }
                        }
                        else if (!Building.Constructing)
                        {
                            Logging.Error(this.GetType(), "GetShortestTaskGO() : Worker allocated to building with remaining construction time 0");
                        }
                        else
                        {
                            RemainingTime = Building.RemainingConstructionTime;
                        }
                    }
                    else if (Construction is Obstacle)
                    {
                        Obstacle Obstacle = (Obstacle)Construction;
                        if (!Obstacle.ClearingOnGoing)
                        {
                            Logging.Error(this.GetType(), "GetShortestTaskGO() : Worker allocated to obstacle with remaining clearing time 0");
                        }
                        else
                        {
                            RemainingTime = Obstacle.RemainingClearingTime;
                        }
                    }
                    else if (Construction is Trap)
                    {
                        Trap Trap = (Trap)Construction;
                        if (!Trap.Constructing)
                        {
                            Logging.Error(this.GetType(), "GetShortestTaskGO() : Worker allocated to Trap with remaining construction time 0");
                        }
                        else
                        {
                            RemainingTime = Trap.RemainingConstructionTime;
                        }
                    }
                    else if (Construction is VillageObject)
                    {
                        VillageObject VillageObject = (VillageObject)Construction;
                        if (!VillageObject.Constructing)
                        {
                            Logging.Error(this.GetType(), "GetShortestTaskGO() : Worker allocated to village object with remaining clearing time 0");
                        }
                        else
                        {
                            RemainingTime = VillageObject.RemainingConstructionTime;
                        }
                    }

                    if (RemainingTime != -1)
                    {
                        if (Task.GameObject == null)
                        {
                            Task.GameObject = Construction;
                            Task.RemainingSeconds = RemainingTime;
                        }
                        else if (Task.RemainingSeconds > RemainingTime)
                        {
                            Task.GameObject = Construction;
                            Task.RemainingSeconds = RemainingTime;
                        }
                    }
                }

                return Task.GameObject;
            }

            return null;
        }

        internal void FinishTaskOfOneWorker()
        {
            GameObject GameObject = this.GetShortestTaskGO();
            if (GameObject != null)
            {
                if (GameObject is Building)
                {
                    Building Building = (Building)GameObject;
                    HeroBaseComponent HeroBaseComponent = Building.HeroBaseComponent;
                    if (Building.Constructing)
                    {
                        Building.SpeedUpConstruction();
                    }
                    else if (HeroBaseComponent != null)
                    {
                        if (HeroBaseComponent.Upgrading)
                        {
                            HeroBaseComponent.SpeedUpUpgrade();
                        }
                    }
                }
                else if (GameObject is Obstacle)
                {
                    Obstacle Obstacle = (Obstacle)GameObject;
                    if (Obstacle.ClearingOnGoing)
                    {
                        Obstacle.SpeedUpClearing();
                    }
                }
                else if (GameObject is Trap)
                {
                    Trap Trap = (Trap)GameObject;
                    if (Trap.Constructing)
                    {
                        Trap.SpeedUpConstruction();
                    }
                }
                else if (GameObject is VillageObject)
                {
                    VillageObject VillageObject = (VillageObject)GameObject;
                    if (VillageObject.Constructing)
                    {
                        VillageObject.SpeedUpConstruction();
                    }
                }
            }
        }

        private struct Task
        {
            internal GameObject GameObject;
            internal int RemainingSeconds;
        }
    }
}