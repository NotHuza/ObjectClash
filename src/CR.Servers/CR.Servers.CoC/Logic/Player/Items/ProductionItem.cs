namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using Newtonsoft.Json.Linq;

    internal class ProductionItem : Item
    {
        internal bool Terminate;

        public ProductionItem()
        {
            // ProductionItem.
        }

        public ProductionItem(Data Data, int Count) : base(Data.GlobalId, Count)
        {
            // ProductionItem.
        }

        public ProductionItem(int Data, int Count) : base(Data, Count)
        {
            // ProductionItem.
        }

        public ProductionItem(int Data, int Count, bool Terminate) : base(Data, Count)
        {
            this.Terminate = Terminate;
        }

        public ProductionItem(Data Data, int Count, bool Terminate) : base(Data.GlobalId, Count)
        {
            this.Terminate = Terminate;
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);
            bool Terminate;
            if (JsonHelper.GetJsonBoolean(Json, "t", out Terminate))
            {
                this.Terminate = Terminate;
            }
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();
            if (this.Terminate)
            {
                Json.Add("t", this.Terminate);
            }

            return Json;
        }
    }
}