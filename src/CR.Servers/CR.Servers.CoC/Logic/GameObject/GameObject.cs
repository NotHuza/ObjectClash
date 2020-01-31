namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using Newtonsoft.Json.Linq;

    internal class GameObject
    {
        internal List<Component> Components;
        internal Data Data;

        internal int Id;
        internal Level Level;
        internal Vector2 Position;

        public GameObject(Data Data, Level Level)
        {
            this.Level = Level;
            this.Data = Data;
            this.Position = new Vector2(0, 0);
            this.Components = new List<Component>(16);
        }

        internal int TileX
        {
            get
            {
                return this.Position.X >> 9;
            }
        }

        internal int TileY
        {
            get
            {
                return this.Position.Y >> 9;
            }
        }

        internal int MidX
        {
            get
            {
                return this.Position.X >> 8;
            }
        }

        internal int MidY
        {
            get
            {
                return this.Position.Y >> 8;
            }
        }

        internal virtual int Checksum
        {
            get
            {
                int Checksum = 0;

                Checksum += this.Type;

                return Checksum;
            }
        }

        internal virtual int HeightInTiles
        {
            get
            {
                return 1;
            }
        }

        internal virtual int WidthInTiles
        {
            get
            {
                return 1;
            }
        }

        internal virtual int VillageType
        {
            get
            {
                return 0;
            }
        }

        internal virtual int Type
        {
            get
            {
                return 0;
            }
        }

        internal void AddComponent(Component Component)
        {
            if (this.Components.Contains(Component))
            {
                Logging.Error(this.GetType(), "AddComponent() : Trying to add a component already added. Type : " + Component.Type + ".");
                return;
            }

            this.Components.Add(Component);

            if (!this.Level.AI)
            {
                this.Level.ComponentManager.AddComponent(Component);
            }
        }

        internal void SetPositionXY(int TileX, int TileY)
        {
            int OldX = this.Position.X >> 9;
            int OldY = this.Position.Y >> 9;

            if (OldX == TileX && OldY == TileX)
            {
                return;
            }

            this.Position.X = TileX << 9;
            this.Position.Y = TileY << 9;
        }

        internal bool TryGetComponent(int Type, out Component Component)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Component component = Components[i];
                if (component.Type == Type)
                {
                    Component = component;
                    return true;
                }
            }

            Component = null;
            return false;
        }

        internal virtual void FastForwardTime(int Secs)
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].FastForwardTime(Secs);
        }

        internal virtual void Tick()
        {
            for (int i = 0; i < Components.Count; i++)
                Components[i].Tick();
        }

        internal virtual void Process()
        {
            // Space
        }

        internal virtual void Load(JToken json)
        {
            int x;
            int y;
            if (JsonHelper.GetJsonNumber(json, "x", out x) && JsonHelper.GetJsonNumber(json, "y", out y))
            {
                this.Position.X = x << 9;
                this.Position.Y = y << 9;
            }
            else
            {
                Logging.Error(this.GetType(), "An error has been throwed when the load of game object. Position X or Y is null!");
            }

            for (int i = 0; i < Components.Count; i++)
                Components[i].Load(json);
        }

        internal virtual void Save(JObject json)
        {
            json.Add("x", this.TileX);
            json.Add("y", this.TileY);

            for (int i = 0; i < Components.Count; i++)
                Components[i].Save(json);
        }
    }
}