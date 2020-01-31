using CR.Servers.CoC.Core;
using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.CoC.Files.CSV_Logic.Logic;
using System.Collections.Generic;

namespace CR.Servers.CoC.Logic
{
    internal class Filter
    {
        internal GameObjectManager GameObjectManager;

        public Filter(GameObjectManager GameObjectManager)
        {
            this.GameObjectManager = GameObjectManager;
        }

        internal List<GameObject> GetGameObjectsByData(Data Data)
        {
            if (Data is BuildingData)
            {
                BuildingData BuildingData = (BuildingData)Data;
                List<GameObject> Result = this.GameObjectManager.GameObjects[0][BuildingData.VillageType].FindAll(T => T.Data.GlobalId == Data.GlobalId);

                return Result;
            }

            return null;
        }

        internal GameObject GetGameObjectByPreciseId(int Id)
        {
            int Class = Id / 1000000 - 500;

            if (Class < GameObjectManager.GameObjects.Length)
            {
                GameObject GameObject = this.GameObjectManager.GameObjects[Class][this.GameObjectManager.Map].Find(g => g.Id == Id);

                if (GameObject != null)
                {
                    return GameObject;
                }

                Logging.Info(this.GetType(), "GameObject id " + Id + " not exist.");
            }

            return null;
        }

        internal GameObject GetGameObjectById(int Id)
        {
            int Class = Id / 1000000 - 500;

            if (this.GameObjectManager.GameObjects.Length > Class)
            {
                int Index = Id % 1000000;

                if (this.GameObjectManager.GameObjects[Class][this.GameObjectManager.Map].Count > Index)
                {
                    return this.GameObjectManager.GameObjects[Class][this.GameObjectManager.Map][Index];
                }

                Logging.Info(this.GetType(), "GameObject id " + Id + " not exist.");
            }

            return null;
        }

        internal int GetGameObjectCount(Data Data, int Level = 0)
        {
            if (Data.GetDataType() == 1)
            {
                int Count = 0;

                foreach (Building GameObject in this.GameObjectManager.GameObjects[0][0])
                {
                    if (GameObject.Data == Data && GameObject.GetUpgradeLevel() >= Level)
                    {
                        ++Count;
                    }
                }

                foreach (Building GameObject in this.GameObjectManager.GameObjects[0][1])
                {
                    if (GameObject.Data == Data && GameObject.GetUpgradeLevel() >= Level)
                    {
                        ++Count;
                    }
                }

                return Count;
            }

            return 0;
        }

        internal GameObject[] GetGameObjects(int Class, int Village)
        {
            return this.GameObjectManager.GameObjects[Class][Village].ToArray();
        }
    }
}