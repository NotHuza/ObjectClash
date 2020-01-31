namespace CR.Servers.CoC.Packets.Commands.Client
{
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Train_Unit_V2 : Command
    {
        internal int BuildingId;
        internal CharacterData Unit;
        internal int UnitOfType;

        public Train_Unit_V2(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 592;
            }
        }

        internal override void Decode()
        {
            this.BuildingId = this.Reader.ReadInt32();
            this.UnitOfType = this.Reader.ReadInt32();
            this.Unit = this.Reader.ReadData<CharacterData>();
            base.Decode();
        }

        internal override void Execute()
        {
            Level Level = this.Device.GameMode.Level;
            GameObject GameObject = Level.GameObjectManager.Filter.GetGameObjectById(this.BuildingId);
            if (GameObject != null)
            {
                if (GameObject is Building)
                {
                    Building Building = (Building)GameObject;
                    UnitStorageV2Component UnitStorageV2Component = Building.UnitStorageV2Component;

                    if (UnitStorageV2Component != null)
                    {
                        UnitStorageV2Component.AddUnit(this.Unit);
                    }
                }
            }
        }
    }
}