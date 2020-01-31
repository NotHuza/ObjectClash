namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Sell_Building : Command
    {
        internal int BuildingId;

        public Sell_Building(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 503;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            GameObject Building = Level.GameObjectManager.Filter.GetGameObjectByPreciseId(this.BuildingId);

            if (Building != null)
            {
                if (Building is Deco)
                {
                    Deco Deco = (Deco)Building;
                    DecoData Data = Deco.DecoData;
                    ResourceData ResourceData = Data.BuildResourceData;

                    if (ResourceData != null)
                    {
                        int SellPrice = Data.SellPrice;
                        if (ResourceData.PremiumCurrency)
                        {
                            Level.Player.AddDiamonds(SellPrice);
                        }
                        else if (Level.Player.Resources.GetCountByData(ResourceData) >= SellPrice)
                        {
                            Level.Player.Resources.Plus(ResourceData.GlobalId, SellPrice);
                            Level.GameObjectManager.RemoveGameObject(Deco, Data.VillageType);
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to sell buidling. The resource data is null!");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to sell buidling. The player tried to sell unsurported buidling!");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to sell buidling. The player tried to sell a null building!");
            }
        }
    }
}