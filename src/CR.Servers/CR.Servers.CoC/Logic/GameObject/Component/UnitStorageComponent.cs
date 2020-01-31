namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using Newtonsoft.Json.Linq;

    internal class UnitStorageComponent : Component
    {
        internal int HousingSpace;
        internal int HousingSpaceAlt;
        internal bool IsSpellForge;

        internal List<Item> Units;

        public UnitStorageComponent(GameObject GameObject) : base(GameObject)
        {
            this.Units = new List<Item>();
            this.SetStorage();
        }

        internal int TotalUsedHousing
        {
            get
            {
                int Housing = 0;

                foreach (Item Unit in this.Units)
                {
                    if (Unit.Count > 0)
                    {
                        Data Data = Unit.ItemData;

                        if (Data.GetDataType() == 4)
                        {
                            Housing += ((CharacterData) Data).HousingSpace * Unit.Count;
                        }
                    }
                }

                return Housing;
            }
        }

        internal int TotalUsedAltHousing
        {
            get
            {
                int Housing = 0;

                foreach (Item Unit in this.Units)
                {
                    if (Unit.Count > 0)
                    {
                        Data Data = Unit.ItemData;

                        if (Data.GetDataType() == 26)
                        {
                            Housing += ((SpellData) Data).HousingSpace * Unit.Count;
                        }
                    }
                }

                return Housing;
            }
        }

        internal override int Type
        {
            get
            {
                return 0;
            }
        }

        internal void AddUnit(Data Data)
        {
            if (Data != null)
            {
                if (this.CanAddUnit(Data))
                {
                    Item Unit = this.Units.Find(T => T.Data == Data.GlobalId);

                    if (Unit != null)
                    {
                        ++Unit.Count;
                    }
                    else
                    {
                        this.Units.Add(new Item(Data.GlobalId, 1));
                    }
                }
                else
                {
                    Logging.Info(this.GetType(), "AddUnit called and storage is full.");
                }
            }
            else
            {
                Logging.Info(this.GetType(), "AddUnit called with CharacterData NULL.");
            }
        }

        internal bool CanAddUnit(Data Data)
        {
            if (Data.GetDataType() == 4)
            {
                if (!this.IsSpellForge)
                {
                    CharacterData Character = (CharacterData) Data;

                    if (this.HousingSpace >= this.TotalUsedHousing + Character.HousingSpace)
                    {
                        return true;
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), $"Unable to add a Character to a spell forge");
                }
            }
            else
            {
                if (this.IsSpellForge)
                {
                    SpellData Spell = (SpellData) Data;

                    if (this.HousingSpace >= this.TotalUsedHousing + Spell.HousingSpace)
                    {
                        return true;
                    }
                }
                else
                {
                    SpellData Spell = (SpellData) Data;

                    if (this.HousingSpaceAlt >= this.TotalUsedAltHousing + Spell.HousingSpace)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal void SetStorage()
        {
            Building Building = (Building) this.Parent;

            if (!Building.Locked)
            {
                int Level = Building.GetUpgradeLevel();

                if (Level >= 0)
                {
                    this.HousingSpace = Building.BuildingData.HousingSpace[Level];
                    this.HousingSpaceAlt = Building.BuildingData.HousingSpaceAlt[Level];
                }
            }

            this.IsSpellForge = Building.BuildingData.IsSpellForge || Building.BuildingData.IsMiniSpellForge;
        }

        internal override void Load(JToken Json)
        {
            JArray Units = (JArray) Json["units"];

            if (Units != null)
            {
                foreach (JToken Unit in Units)
                {
                    int[] Array = Unit.ToObject<int[]>();

                    int ID = Array[0];
                    int Count = Array[1];

                    if (ID != 0)
                    {
                        this.Units.Add(new Item(ID, Count));
                    }
                }
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            JArray Units = new JArray();

            foreach (Item unit in this.Units)
            {
                Units.Add(new JArray
                {
                    unit.Data,
                    unit.Count
                });
            }

            Json.Add("units", Units);
            base.Save(Json);
        }
    }
}