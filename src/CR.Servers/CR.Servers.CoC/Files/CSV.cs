namespace CR.Servers.CoC.Files
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Files.CSV_Reader;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Files.CSV_Reader;

    internal static class CSV
    {
        internal static readonly Dictionary<int, string> Gamefiles = new Dictionary<int, string>();

        internal static Gamefiles Tables;
        internal static CharacterData Village2StartUnit;
        internal static BuildingData TownHallData;
        internal static BuildingData TownHallVillage2Data;
        internal static BuildingData AllianceCastleData;

        internal static void Initialize()
        {
            CSV.Tables = new Gamefiles();

            CSV.Gamefiles.Add((int) Gamefile.Buildings, @"Gamefiles/logic/buildings.csv");
            CSV.Gamefiles.Add((int) Gamefile.Locales, @"Gamefiles/logic/locales.csv");
            CSV.Gamefiles.Add((int) Gamefile.Resources, @"Gamefiles/logic/resources.csv");
            CSV.Gamefiles.Add((int) Gamefile.Characters, @"Gamefiles/logic/characters.csv");
            CSV.Gamefiles.Add((int) Gamefile.Building_Classes, @"Gamefiles/logic/building_classes.csv");
            CSV.Gamefiles.Add((int) Gamefile.Obstacles, @"Gamefiles/logic/obstacles.csv");
            CSV.Gamefiles.Add((int) Gamefile.Traps, @"Gamefiles/logic/traps.csv");
            CSV.Gamefiles.Add((int) Gamefile.Globals, @"Gamefiles/logic/globals.csv");
            CSV.Gamefiles.Add((int) Gamefile.Experience_Levels, @"Gamefiles/logic/experience_levels.csv");
            CSV.Gamefiles.Add((int) Gamefile.Townhall_Levels, @"Gamefiles/logic/townhall_levels.csv");
            CSV.Gamefiles.Add((int) Gamefile.Npcs, @"Gamefiles/logic/npcs.csv");
            CSV.Gamefiles.Add((int) Gamefile.Decos, @"Gamefiles/logic/decos.csv");
            CSV.Gamefiles.Add((int) Gamefile.Shields, @"Gamefiles/logic/shields.csv");
            CSV.Gamefiles.Add((int) Gamefile.Missions, @"Gamefiles/logic/missions.csv");
            CSV.Gamefiles.Add((int) Gamefile.Achievements, @"Gamefiles/logic/achievements.csv");
            CSV.Gamefiles.Add((int) Gamefile.Spells, @"Gamefiles/logic/spells.csv");
            CSV.Gamefiles.Add((int) Gamefile.Heroes, @"Gamefiles/logic/heroes.csv");
            CSV.Gamefiles.Add((int) Gamefile.Variables, @"Gamefiles/logic/variables.csv");
            CSV.Gamefiles.Add((int) Gamefile.Leagues, @"Gamefiles/logic/leagues.csv");
            CSV.Gamefiles.Add((int) Gamefile.Regions, @"Gamefiles/logic/regions.csv");
            CSV.Gamefiles.Add((int) Gamefile.AllianceBadgeLayer, @"Gamefiles/logic/alliance_badge_layers.csv");
            CSV.Gamefiles.Add((int) Gamefile.Village_Objects, @"Gamefiles/logic/village_objects.csv");

            foreach (KeyValuePair<int, string> File in CSV.Gamefiles)
            {
                if (new FileInfo(File.Value).Exists)
                {
                    CSV.Tables.Initialize(new Table(File.Value), File.Key);
                }
                else
                {
                    throw new FileNotFoundException($"{File.Value} does not exist!");
                }
            }

            foreach (DataTable table in CSV.Tables.DataTables.Values)
            {
                foreach (Data data in table.Datas)
                {
                    data.Process();
                }
            }

            CSV.Village2StartUnit = (CharacterData) CSV.Tables.Get(Gamefile.Characters).GetData(((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("VILLAGE2_START_UNIT")).TextValue);

            List<Data> Buildings = CSV.Tables.Get(Gamefile.Buildings).Datas;

            for (int i = 0; i < Buildings.Count; i++)
            {
                BuildingData buildingData = (BuildingData) Buildings[i];

                if (buildingData.IsAllianceCastle)
                {
                    CSV.AllianceCastleData = buildingData;
                }
                else if (buildingData.IsTownHall)
                {
                    CSV.TownHallData = buildingData;
                }

                else if(buildingData.IsTownHall2)
                {
                    CSV.TownHallVillage2Data = buildingData;
                }
            }
            
            StringBuilder Help = new StringBuilder();
            Help.AppendLine("Clashers Republic - AI Base Generator");
            Help.AppendLine("Available Building:");

            foreach (Data Building in Buildings)
            {
                if (Building is BuildingData)
                {
                    var BuildingData = (BuildingData)Building;
                    Help.AppendLine($" ID {BuildingData.InstanceId}\n Name: {BuildingData.Name}\n Alt Mode: {!string.IsNullOrEmpty(BuildingData.GearUpBuilding) || BuildingData.AltAttackMode}\n");
                }
            }


            Help.AppendLine("Command:\n/AIBase {Id-Here} {Attack-Mode-Here-If Available (Optional)}");

            Help.AppendLine("Example:");
            Help.AppendLine(" /AIBase 1 (Will generate normal town hall)\n/AIBase 1 true (Will not generate anything due to Alt Mode is not avaiable)\n/AIBase 9      (Will generate normal archer tower)\n/AIBase 9 true (Will generate fast attack archer tower)");
            Constants.AIBaseHelp = Help;

            Console.WriteLine(CSV.Gamefiles.Count + " CSV Files loaded and stored in memory.");
        }
    }
}