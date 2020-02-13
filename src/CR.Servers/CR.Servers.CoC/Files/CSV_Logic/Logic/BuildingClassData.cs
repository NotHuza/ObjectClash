using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.Files.CSV_Reader;

namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class BuildingClassData : Data
    {
        public BuildingClassData(Row rw, DataTable dt) : base(rw, dt)
        {
        }

        public override string Name { get; set; }

        public string TID { get; set; }

        public bool CanBuy { get; set; }

        public bool ShopCategoryResource { get; set; }

        public bool ShopCategoryArmy { get; set; }

        public bool ShopCategoryDefense { get; set; }
    }
}
