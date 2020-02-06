using System;
using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.CoC.Logic.Enums;
using CR.Servers.Files.CSV_Reader;

namespace CR.Servers.CoC.Files.CSV_Helpers
{
    internal class DataTables
    {
        internal BuidilngData GetBuildingByName(string str)
        {
            return (BuildingData) CSV.Tables.Get(Gamefile.Buildings).GetData(str);
        }

        internal ResourceData GetResourceByName(string str)
        {
            return (ResourceData) CSV.Tables.Get(Gamefile.Resources).GetData(str);
        }

        internal CharacterData GetCharacterByName(string str)
        {
            return (ResourceData) CSV.Tables.Get(Gamefile.Characters).GetData(str);
        }

        internal BuildingClassData GetBuildingClassByName(string str)
        {
            return (BuildingClassData) CSV.Tables.Get(Gamefile.Building_Classes).GetData(str);
        }

        internal ObstacleData GetObstacleByName(string str)
        {
            return (ObstacleData) CSV.Tables.Get(Gamefile.Obstacles).GetData(str);
        }

        internal GlobalData GetGlobalByName(string str)
        {
            return (GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData(str);
        }
 
        internal EffectData GetEffectByName(string str)
        {
            return (EffectData) CSV.Tables.Get(Gamefile.Effects).GetData(str);
        }

        internal SpellData GetSpellByName(string str)
        {
            return (SpellData) CSV.Tables.Get(Gamefile.Spells).GetData(str);
        }
    }
}
