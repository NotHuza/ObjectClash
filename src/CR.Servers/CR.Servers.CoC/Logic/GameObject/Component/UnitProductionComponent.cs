namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using Newtonsoft.Json.Linq;

    internal class UnitProductionComponent : Component
    {
        internal bool IsSpellForge;

        public UnitProductionComponent(GameObject go) : base(go)
        {
            this.SetProductionType(go);
        }

        internal override int Type
        {
            get
            {
                return 3;
            }
        }

        public void SetProductionType(GameObject go)
        {
            GameObject b = this.Parent;
            BuildingData bd = (BuildingData) b.Data;
            this.IsSpellForge = bd.IsSpellForge || bd.IsMiniSpellForge;
        }

        internal override void Load(JToken Json)
        {
            JObject UnitProd = (JObject) Json["unit_prod"];
            if (UnitProd != null)
            {
                bool IsSpellForge;
                if (JsonHelper.GetJsonBoolean(UnitProd, "unit_type", out IsSpellForge))
                {
                    this.IsSpellForge = IsSpellForge;
                }
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            JObject UnitProd = new JObject
            {
                {
                    "m", 1
                },
                {
                    "unit_type", this.IsSpellForge ? 1 : 0
                }
            };

            Json.Add("unit_prod", UnitProd);
            base.Save(Json);
        }
    }
}