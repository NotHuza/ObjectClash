namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;

    internal class Deco : GameObject
    {
        public Deco(Data Data, Level Level) : base(Data, Level)
        {
        }

        internal DecoData DecoData
        {
            get
            {
                return (DecoData) this.Data;
            }
        }

        internal override int HeightInTiles
        {
            get
            {
                return this.DecoData.Height;
            }
        }

        internal override int WidthInTiles
        {
            get
            {
                return this.DecoData.Width;
            }
        }

        internal override int Type
        {
            get
            {
                return 6;
            }
        }

        internal override int VillageType
        {
            get
            {
                return this.DecoData.VillageType;
            }
        }
    }
}