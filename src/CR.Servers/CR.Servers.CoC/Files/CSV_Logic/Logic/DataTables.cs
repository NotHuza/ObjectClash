using System;
using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.CoC.Logic.Enums;
using CR.Servers.Files.CSV_Reader;

namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class DataTables
    {
        internal static BuidilngData GetBuildingByName(string str)
        {
            return (BuildingData) CSV.Tables.Get(Gamefile.Buildings).GetData(str);
        }

        internal static ResourceData GetResourceByName(string str)
        {
            return (ResourceData) CSV.Tables.Get(Gamefile.Resources).GetData(str);
        }

        internal static CharacterData GetCharacterByName(string str)
        {
            return (ResourceData) CSV.Tables.Get(Gamefile.Characters).GetData(str);
        }

        internal static BuildingClassData GetBuildingClassByName(string str)
        {
            return (BuildingClassData) CSV.Tables.Get(Gamefile.Building_Classes).GetData(str);
        }

        internal static ObstacleData GetObstacleByName(string str)
        {
            return (ObstacleData) CSV.Tables.Get(Gamefile.Obstacles).GetData(str);
        }

        internal static GlobalData GetGlobalByName(string str)
        {
            return (GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData(str);
        }
 
        internal static EffectData GetEffectByName(string str)
        {
            return (EffectData) CSV.Tables.Get(Gamefile.Effects).GetData(str);
        }

        internal static SpellData GetSpellByName(string str)
        {
            return (SpellData) CSV.Tables.Get(Gamefile.Spells).GetData(str);
        }

        internal static VariableData GetVariableByName(string str)
        {
            return (VariableData) CSV.Tables.Get(Gamefile.Variables).GetData(str);
        }
    }
}
