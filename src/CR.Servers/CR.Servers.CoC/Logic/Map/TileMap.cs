#define VERBOSE

#if VERBOSE
using CR.Servers.CoC.Core;
#endif
using System.Collections.Generic;

namespace CR.Servers.CoC.Logic.Map
{
    internal class TileMap
    {
        private Tile[][] Tiles;

        public TileMap(int Width, int Height)
        {
            this.Tiles = new Tile[2][];

            this.Tiles[0] = new Tile[Width * Height];
            this.Tiles[1] = new Tile[Width * Height];

            for (int i = 0; i < Width * Height; i++)
            {
                this.Tiles[0][i] = new Tile(new List<GameObject>(4));
                this.Tiles[1][i] = new Tile(new List<GameObject>(4));
            }
        }

        internal Tile this[int X, int Y, int Map]
        {
            get
            {
                int Index = 50 * X + Y;
                if (50 * 50 > Index && Index >= 0)
                    return this.Tiles[Map][Index];

#if VERBOSE
                Logging.Error(typeof(TileMap), "Unable to find tile -> use TileMap.Exists before?");
#endif
                return default(Tile);
            }
        }

        internal bool Exists(int X, int Y, int Map)
        {
            int Index = 50 * X + Y;
            if (50 * 50 > Index && Index >= 0)
                return true;
            return false;
        }

        internal void AddGameObject(GameObject GameObject)
        {
            for (int i = 0; i < GameObject.WidthInTiles; i++)
            {
                for (int j = 0; j < GameObject.HeightInTiles; j++)
                {
                    this.Tiles[GameObject.VillageType][50 * (i + GameObject.TileX) + j + GameObject.TileY].AddGameObject(GameObject);
                }
            }
        }

        internal void AddGameObject(GameObject GameObject, int VillageType)
        {
            for (int i = 0; i < GameObject.WidthInTiles; i++)
            {
                for (int j = 0; j < GameObject.HeightInTiles; j++)
                {
                    this.Tiles[VillageType][50 * (i + GameObject.TileX) + j + GameObject.TileY].AddGameObject(GameObject);
                }
            }
        }

        internal void GameObjectMoved(GameObject GameObject, int OldX, int OldY)
        {
            for (int i = 0; i < GameObject.WidthInTiles; i++)
            {
                for (int j = 0; j < GameObject.HeightInTiles; j++)
                {
                    this.Tiles[GameObject.VillageType][50 * (i + OldX) + j + OldY].RemoveGameObject(GameObject);
                }
            }

            for (int i = 0; i < GameObject.WidthInTiles; i++)
            {
                for (int j = 0; j < GameObject.HeightInTiles; j++)
                {
                    this.Tiles[GameObject.VillageType][50 * (i + GameObject.TileX) + j + GameObject.TileY].AddGameObject(GameObject);
                }
            }
        }

        internal List<Tile> GetTile(GameObject GameObject, int X, int Y, int Map)
        {
            List<Tile> Tiles = new List<Tile>();
            for (int i = 0; i < GameObject.WidthInTiles; i++)
            {
                for (int j = 0; j < GameObject.HeightInTiles; j++)
                {
                    Tiles.Add(this.Tiles[GameObject.VillageType][50 * (i + X) + j + Y]);
                }
            }

            return Tiles;
        }

        public void Destruct()
        {
            if (this.Tiles != null)
            {
                for (int i = 0; i < this.Tiles.Length; i++)
                {
                    for (int j = 0; j < this.Tiles[j].Length; j++)
                    {
                        this.Tiles[i][j].Destruct();
                    }
                    this.Tiles[i] = null;
                }

                this.Tiles = null;
            }
        }
    }
}