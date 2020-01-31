namespace CR.Servers.CoC.Packets.Commands.Client
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Logic;
    using CR.Servers.Extensions.Binary;

    internal class Collect_Resource : Command
    {
        internal int BuildingId;

        public Collect_Resource(Device device, Reader reader) : base(device, reader)
        {
        }

        internal override int Type
        {
            get
            {
                return 506;
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
            Building Building = (Building) Level.GameObjectManager.Filter.GetGameObjectById(this.BuildingId);

            if (Building != null)
            {
                if (Globals.CollectAllResourcesAtOnce)
                {
                    List<GameObject> Buildings = Level.GameObjectManager.Filter.GetGameObjectsByData(Building.Data);

                    for (int i = 0; i < Buildings.Count; i++)
                    {
                        Building building = (Building) Buildings[i];

                        if (!building.Constructing)
                        {
                            ResourceProductionComponent ResourceProductionComponent = building.ResourceProductionComponent;

                            ResourceProductionComponent?.CollectResources();
                        }
                    }
                }
                else
                {
                    ResourceProductionComponent ResourceProductionComponent = Building.ResourceProductionComponent;

                    ResourceProductionComponent?.CollectResources();
                }
            }
        }
    }
}