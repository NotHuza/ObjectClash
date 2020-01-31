namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Remove_Units_V2 : Command
    {
        internal int BuildingId;

        public Remove_Units_V2(Device Device, Reader Reader) : base(Device, Reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 596;
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
            GameObject Gameobject = Level.GameObjectManager.Filter.GetGameObjectById(this.BuildingId);
            if (Gameobject != null)
            {
                if (Gameobject is Building)
                {
                    Building Building = (Building)Gameobject;
                    UnitStorageV2Component UnitStorageV2Component = Building.UnitStorageV2Component;

                    if (UnitStorageV2Component != null)
                    {
                        Item Unit = Level.Player.Units2.GetByGlobalId(UnitStorageV2Component.Units[0].Data);

                        if (Unit != null)
                        {
                            Unit.Count -= UnitStorageV2Component.Units[0].Count;
                        }

                        UnitStorageV2Component.Units = new List<Item>();
                    }
                    else
                    {
                        Logging.Error(this.GetType(), "Unable to remove unit v2. The UnitStorageV2Component is null!");
                    }
                }
                else
                {
                    Logging.Error(this.GetType(), "Unable to remove unit v2. The gameobject is not a building!");
                }
            }
            else
            {
                Logging.Error(this.GetType(), "Unable to remove unit v2. The gameobject is null!");
            }
        }
    }
}