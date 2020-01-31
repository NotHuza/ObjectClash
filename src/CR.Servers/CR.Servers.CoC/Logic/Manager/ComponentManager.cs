namespace CR.Servers.CoC.Logic
{
    using System;
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Enums;

    internal class ComponentManager
    {
        internal List<Component>[] Components;
        internal Level Level;

        public ComponentManager(Level Level)
        {
            this.Level = Level;
            this.Components = new List<Component>[20];

            for (int i = 0; i < 20; i++)
            {
                this.Components[i] = new List<Component>();
            }
        }


        internal List<Building> TroopHousings
        {
            get
            {
                List<Building> TroopHousings = new List<Building>(8);

                var GameObjects = this.Level.GameObjectManager.GameObjects[0][0].ToArray();

                for (int i = 0; i < GameObjects.Length; i++)
                {
                    Building Building = (Building) GameObjects[i];

                    if (Building.BuildingData.IsTrainingHousing)
                    {
                        if (Building.GetUpgradeLevel() > -1)
                        {
                            TroopHousings.Add(Building);
                        }
                    }
                }

                return TroopHousings;
            }
        }

        internal List<Building> DarkSpellForge
        {
            get
            {
                List<Building> DarkSpellForge = new List<Building>();

                var GameObjects = this.Level.GameObjectManager.GameObjects[0][0].ToArray();

                for (int i = 0; i < GameObjects.Length; i++)
                {
                    Building Building = (Building) GameObjects[i];

                    if (Building.BuildingData.IsSpellForge && Building.BuildingData.IsMiniSpellForge)
                    {
                        if (Building.GetUpgradeLevel() > -1)
                        {
                            DarkSpellForge.Add(Building);
                        }
                    }
                }

                return DarkSpellForge;
            }
        }


        internal List<Building> DarkBarracks
        {
            get
            {
                List<Building> DarkBarracks = new List<Building>();
                var GameObjects = this.Level.GameObjectManager.GameObjects[0][0].ToArray();

                for (int i = 0; i < GameObjects.Length; i++)
                {
                    Building Building = (Building) GameObjects[i];

                    if (Building.BuildingData.IsDarkBarrack)
                    {
                        if (Building.GetUpgradeLevel() > -1)
                        {
                            DarkBarracks.Add(Building);
                        }
                    }
                }

                return DarkBarracks;
            }
        }

        internal List<Building> SpellForge
        {
            get
            {
                List<Building> SpellForge = new List<Building>();

                var GameObjects = this.Level.GameObjectManager.GameObjects[0][0].ToArray();

                for (int i = 0; i < GameObjects.Length; i++)
                {
                    Building Building = (Building) GameObjects[i];
                    if (Building.BuildingData.IsSpellForge && !Building.BuildingData.IsMiniSpellForge)
                    {
                        if (Building.GetUpgradeLevel() > -1)
                        {
                            SpellForge.Add(Building);
                        }
                    }
                }

                return SpellForge;
            }
        }

        internal List<Building> Barracks
        {
            get
            {
                List<Building> Barracks = new List<Building>();
                var GameObjects = this.Level.GameObjectManager.GameObjects[0][0].ToArray();

                for (int i = 0; i < GameObjects.Length; i++)
                {
                    Building Building = (Building) GameObjects[i];

                    if (Building.BuildingData.IsBarrack)
                    {
                        if (Building.GetUpgradeLevel() > -1)
                        {
                            Barracks.Add(Building);
                        }
                    }
                }

                return Barracks;
            }
        }

        internal int TotalMaxSpellHousing
        {
            get
            {
                int Count = 0;

                for (int i = 0; i < this.Components[0].Count; i++)
                {
                    var Component = this.Components[0][i];
                    if (Component.Type == 0)
                    {
                        UnitStorageComponent Storage = (UnitStorageComponent) Component;
                        if (Storage.IsSpellForge)
                        {
                            Count += ((UnitStorageComponent) Component).HousingSpace;
                        }
                    }
                }

                return Count;
            }
        }


        internal int TotalMaxHousing
        {
            get
            {
                int Count = 0;

                for (int i = 0; i < this.Components[0].Count; i++)
                {
                    var Component = this.Components[0][i];

                    if (Component.Type == 0)
                    {
                        UnitStorageComponent Storage = (UnitStorageComponent) Component;
                        if (!Storage.IsSpellForge)
                        {
                            Count += ((UnitStorageComponent) Component).HousingSpace;
                        }
                    }
                }

                return Count;
            }
        }

        internal int MaxBarrackLevel
        {
            get
            {
                int MaxLevel = -1;

                for (int i = 0; i < this.Components[0].Count; i++)
                {
                    Building Building = (Building) this.Components[0][i].Parent;

                    if (Building.BuildingData.IsBarrack)
                    {
                        MaxLevel = Math.Max(MaxLevel, Building.GetUpgradeLevel());
                    }
                }

                return MaxLevel;
            }
        }

        internal int MaxSpellForgeLevel
        {
            get
            {
                int MaxLevel = -1;

                for (int i = 0; i < this.Components[0].Count; i++)
                {
                    Building Building = (Building)this.Components[0][i].Parent;

                    if (Building.BuildingData.IsSpellForge && !Building.BuildingData.IsMiniSpellForge)
                    {
                        MaxLevel = Math.Max(MaxLevel, Building.GetUpgradeLevel());
                    }
                }
               
                return MaxLevel;
            }
        }

        internal int MaxDarkBarrackLevel
        {
            get
            {
                int MaxLevel = -1;

                for (int i = 0; i < this.Components[0].Count; i++)
                {
                    Building Building = (Building)this.Components[0][i].Parent;

                    if (Building.BuildingData.IsDarkBarrack)
                    {
                        MaxLevel = Math.Max(MaxLevel, Building.GetUpgradeLevel());
                    }
                }

                return MaxLevel;
            }
        }

        internal int MaxDarkSpellForgeLevel
        {
            get
            {
                int MaxLevel = -1;

                for (int i = 0; i < this.Components[0].Count; i++)
                {
                    Building Building = (Building)this.Components[0][i].Parent;

                    if (Building.BuildingData.IsSpellForge && Building.BuildingData.IsMiniSpellForge)
                    {
                        MaxLevel = Math.Max(MaxLevel, Building.GetUpgradeLevel());
                    }
                }

                return MaxLevel;
            }
        }

        internal void AddComponent(Component Component)
        {
            this.Components[Component.Parent.Type].Add(Component);
        }

        internal void RemoveComponent(Component Component)
        {
            this.Components[Component.Parent.Type].Remove(Component);
        }

        internal void FindAll(Predicate<Component> Match)
        {
            List<Component> Components = new List<Component>(16);

            for (int i = 0; i < this.Components[0].Count; i++)
            {
                if (Match(this.Components[0][i]))
                {
                    Components.Add(this.Components[0][i]);
                }
            }

            for (int i = 0; i < this.Components[1].Count; i++)
            {
                if (Match(this.Components[1][i]))
                {
                    Components.Add(this.Components[1][i]);
                }
            }
        }

        internal List<Component> FindAll(int GameObjectType, Predicate<Component> Match)
        {
            List<Component> Components = new List<Component>(16);
            for (int i = 0; i < this.Components[GameObjectType].Count; i++)
            {

                if (Match(this.Components[GameObjectType][i]))
                {
                    Components.Add(this.Components[GameObjectType][i]);
                }

            }

            return Components;
        }

        internal void RefreshResourceCaps()
        {
            if (this.Level.Player != null)
            {
                Player Player = this.Level.Player;
                
                for (int i = 0; i < Player.ResourceCaps.Count; i++)
                {
                    Player.ResourceCaps[i].Count = 0;
                }

                for (int i = 0; i < this.Components[0].Count; i++)
                {
                    if (this.Components[0][i].Type == 6)
                    {
                        ResourceStorageComponent StorageComponent = (ResourceStorageComponent) this.Components[0][i];

                        foreach (ResourceData Data in CSV.Tables.Get(Gamefile.Resources).Datas)
                        {
                            Player.ResourceCaps.Add(Data, StorageComponent.GetMax(Data.GetId()));
                        }
                    }
                    
                }
            }
            else
            {
                Logging.Info(this.GetType(), "Unable to refresh resource caps. Player is NULL.");
            }
        }

        internal static Component GetClosestComponent(int X, int Y, List<Component> Components)
        {
            Component Closest = null;
            int ClosestDistance = 0;

            for (int i = 0; i < Components.Count; i++)
            {
                if (Closest == null || ClosestDistance > Components[i].Parent.Position.GetDistanceSquaredHelper(X, Y))
                {
                    Closest = Components[i];
                }
            }

            return Closest;
        }
    }
}