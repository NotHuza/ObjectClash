namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Buy_Deco : Command
    {
        internal DecoData DecoData;

        internal int X;
        internal int Y;

        public Buy_Deco(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 512;
            }
        }


        internal override void Decode()
        {
            this.X = this.Reader.ReadInt32();
            this.Y = this.Reader.ReadInt32();

            this.DecoData = this.Reader.ReadData<DecoData>();

            base.Decode();
        }

        internal override void Execute()
        {
            if (this.DecoData != null)
            {
                Level Level = this.Device.GameMode.Level;
                //TODO Add Cap check
                //if (!Level.IsBuildingCapReached(this.BuildingData))
                {
                    ResourceData ResourceData = this.DecoData.BuildResourceData;
                    if (this.DecoData.RequiredExpLevel <= Level.Player.ExpLevel)
                    {
                        if (ResourceData.PremiumCurrency)
                        {
                            if (Level.Player.HasEnoughDiamonds(this.DecoData.BuildCost))
                            {
                                Level.Player.UseDiamonds(this.DecoData.BuildCost);
                                this.AddGameObject(Level);
                            }
                        }
                        else if (Level.Player.Resources.GetCountByData(ResourceData) >= this.DecoData.BuildCost)
                        {
                            Level.Player.Resources.Remove(ResourceData, this.DecoData.BuildCost);
                            this.AddGameObject(Level);
                        }
                        else
                        {
                            Logging.Error(this.GetType(), "Unable to buy deco. The player doesn't have enough resources!");
                        }
                    }
                    else
                    {
                        Logging.Error(this.GetType(), $"Unable to buy deco. The player have exp level lower then required exp level ({Level.Player.ExpLevel} - {this.DecoData.RequiredExpLevel})!");
                    }
                }
            }
        }

        internal void AddGameObject(Level Level)
        {
            Deco GameObject = new Deco(this.DecoData, Level)
            {
                Position =
                {
                    X = this.X << 9,
                    Y = this.Y << 9
                },
                Id = GlobalId.Create(506, Level.GameObjectManager.DecoIndex[this.DecoData.VillageType]++)
            };

            Level.GameObjectManager.GameObjects[6][this.DecoData.VillageType].Add(GameObject);
        }
    }
}