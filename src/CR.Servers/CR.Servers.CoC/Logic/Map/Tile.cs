namespace CR.Servers.CoC.Logic.Map
{
    using System.Collections.Generic;

    internal struct Tile
    {
        internal List<GameObject> GameObjects;

        public Tile(List<GameObject> gameObjects)
        {
            this.GameObjects = gameObjects;
        }

        internal void AddGameObject(GameObject GameObject)
        {
            if (this.GameObjects.Contains(GameObject))
                return;

            this.GameObjects.Add(GameObject);
        }

        internal bool IsBuildable()
        {
            return this.GameObjects.Count == 0;
        }

        internal void RemoveGameObject(GameObject GameObject)
        {
            this.GameObjects.Remove(GameObject);
        }

        public void Destruct()
        {
            if (this.GameObjects != null)
            {
                if (this.GameObjects.Count != 0)
                {
                    do
                    {
                        this.GameObjects.RemoveAt(0);
                    } while (this.GameObjects.Count != 0);
                }

                this.GameObjects = null;
            }
        }
    }
}