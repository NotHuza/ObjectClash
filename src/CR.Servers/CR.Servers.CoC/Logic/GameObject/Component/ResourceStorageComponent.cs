namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Core;

    internal class ResourceStorageComponent : Component
    {
        internal int[] MaxArray;

        public ResourceStorageComponent(GameObject GameObject) : base(GameObject)
        {
            // ResourceStorageComponent.
        }

        internal override int Type
        {
            get
            {
                return 6;
            }
        }

        internal int GetMax(int ResourceID)
        {
            if (ResourceID > this.MaxArray.Length)
            {
                Logging.Info(this.GetType(), "Index is out of range : " + ResourceID);
                return 0;
            }

            return this.MaxArray[ResourceID];
        }

        internal void SetMaxArray(int[] MaxArray)
        {
            this.MaxArray = MaxArray;
        }
    }
}