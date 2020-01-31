namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using Newtonsoft.Json.Linq;

    internal class UnitStorageV2Component : Component
    {
        internal int HousingSpace;
        internal int UnitInCamp;

        internal List<Item> Units;

        public UnitStorageV2Component(GameObject GameObject) : base(GameObject)
        {
            this.Units = new List<Item>();
            this.SetStorage();
        }

        internal override int Type
        {
            get
            {
                return 11;
            }
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

        internal void SetStorage()
        {
            Building Building = (Building) this.Parent;

            if (!Building.Locked)
            {
                int Level = Building.GetUpgradeLevel();

                if (Level >= 0)
                {
                    this.HousingSpace = Building.BuildingData.Village2Housing;
                }
            }
        }

        internal void AddUnit(Data Data)
        {
            if (Data != null)
            {
                if (this.CanAddUnit(Data))
                {
                    CharacterData Character = (CharacterData) Data;
                    int Level = this.Parent.Level.Player.GetUnitUpgradeLevel(Character);

                    Item Unit = this.Units.Find(T => T.Data == Data.GlobalId);
                    if (Unit != null)
                    {
                        ++Unit.Count;
                    }
                    else
                    {
                        this.UnitInCamp += Character.UnitsInCamp[Level];
                        this.Units.Add(new Item(Data.GlobalId, Character.UnitsInCamp[Level]));
                        this.Parent.Level.Player.Units2.Add(Character, Character.UnitsInCamp[Level]);
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
                CharacterData Character = (CharacterData) Data;
                if (this.HousingSpace >= this.TotalUsedHousing + Character.HousingSpace)
                {
                    return true;
                }
            }

            return false;
        }

        internal override void Load(JToken Json)
        {
            JObject units = (JObject) Json["up2"];
            JArray unitsArray = (JArray) units?["unit"];

            if (unitsArray?.Count > 0)
            {
                int[] Array = unitsArray.ToObject<int[]>();

                int ID = Array[0];
                int Count = Array[1];

                if (ID > 0)
                {
                    this.Units.Add(new Item(ID, Count));
                }
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            JObject units = new JObject();
            JArray unitsArray = new JArray();

            if (this.Units.Count > 0)
            {
                foreach (Item unit in this.Units)
                {
                    unitsArray = new JArray
                    {
                        unit.Data,
                        unit.Count
                    };
                }

                units.Add("unit", unitsArray);
            }

            Json.Add("up2", units);
            base.Save(Json);
        }
    }
}