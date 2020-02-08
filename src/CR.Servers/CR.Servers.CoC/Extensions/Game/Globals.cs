using CR.Servers.CoC.Files;
using CR.Servers.CoC.Files.CSV_Logic.Logic;
using CR.Servers.CoC.Logic.Enums;

﻿namespace CR.Servers.CoC.Extensions.Game
{
    internal class Globals
    {
        internal static bool CollectAllResourcesAtOnce;

        internal static int StartingGold;
        internal static int StartingElixir;
        internal static int StartingGold2;
        internal static int StartingElixir2;
        internal static int StartingDiamonds;
        internal static int AllianceCreateCost;

        internal static int SpeedUpDiamondCost1Min;
        internal static int SpeedUpDiamondCost1Hour;
        internal static int SpeedUpDiamondCost24Hours;
        internal static int SpeedUpDiamondCost1Week;
        internal static int Village2SpeedUpDiamondCost1Min;
        internal static int Village2SpeedUpDiamondCost1Hour;
        internal static int Village2SpeedUpDiamondCost24Hours;
        internal static int Village2SpeedUpDiamondCost1Week;

        internal static int ResourceDiamondCost100;
        internal static int ResourceDiamondCost1000;
        internal static int ResourceDiamondCost10000;
        internal static int ResourceDiamondCost100000;
        internal static int ResourceDiamondCost1000000;
        internal static int ResourceDiamondCost10000000;

        internal static int DarkElixirDiamondCost1;
        internal static int DarkElixirDiamondCost10;
        internal static int DarkElixirDiamondCost100;
        internal static int DarkElixirDiamondCost1000;
        internal static int DarkElixirDiamondCost10000;
        internal static int DarkElixirDiamondCost100000;

        internal static int Village2ResourceDiamondCost100;
        internal static int Village2ResourceDiamondCost1000;
        internal static int Village2ResourceDiamondCost10000;
        internal static int Village2ResourceDiamondCost100000;
        internal static int Village2ResourceDiamondCost1000000;
        internal static int Village2ResourceDiamondCost10000000;

        internal static int TroopTrainingSpeedUpTutorialCost;

        internal static int HeroHealthSpeedUpCostMultiplier;
        internal static int SpellSpeedUpCostMultiplier;
        internal static int TroopRequestSpeedUpCostMultiplier;
        internal static int TrainCancelMultiplier;
        internal static int BuildCancelMultiplier;
        internal static int SpellCancelMultiplier;
        internal static int HeroUpgradeCancelMultiplier;

        internal static int WorkerCost2Nd;
        internal static int WorkerCost3Rd;
        internal static int WorkerCost4Th;
        internal static int WorkerCost5Th;

        internal static int AllianceTroopRequestCooldown;
        internal static int ClanMailCooldown;
        internal static int ReplayShareCooldown;
        internal static int ElderKickCooldown;
        internal static int ChallengeCooldown;
        internal static int ArrangeWarCooldown;

        internal static int ObstacleCountMax;
        internal static int ObstacleRespawnSeconds;

        internal static ResourceData AllianceCreateResourceData;

        internal static int[] TroopHousingV2Cost;
        internal static int[] TroopHousingV2BuildTimeSeconds;
        internal static int[] LootMultiplierByTownHallDifference;
        internal static int[] BarrackReduceTrainingDivisor;
        internal static int[] DarkBarrackReduceTrainingDivisor;
        internal static int[] ClockTowerBoostMins;

        internal static int Village2DoNotAllowClearObstacleTh;
        internal static int TallGrassRespawnSeconds;

        internal static int AttackLength;
        internal static int AttackPrepartionLength;
        internal static int AttackPrepartionLength2;

        internal static GlobalData GetGlobalData(string str)
        {
            return DataTables.Csv14Name(str);
        }

        internal static int GlobalNumberValue(string str)
        {
            return this.GetGlobalData(str).NumberValue;
        }

        internal bool GlobalBooleanValue(string str)
        {
            return this.GetGlobalData(str).BooleanValue;
        }

        internal static void Initialize()
        {
            Globals.AllianceCreateResourceData = (ResourceData) CSV.Tables.Get(Gamefile.Resources).GetData(((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("ALLIANCE_CREATE_RESOURCE")).TextValue);
            Globals.AllianceCreateCost = Globals.GlobalNumberValue("ALLIANCE_CREATE_COST");
            Globals.StartingDiamonds = Globals.GlobalNumberValue("STARTING_DIAMONDS");
            Globals.StartingGold = Globals.GlobalNumberValue("STARTING_GOLD");
            Globals.StartingElixir = Globals.GlobalNumberValue("STARTING_ELIXIR");
            Globals.StartingGold2 = Globals.GlobalNumberValue("STARTING_GOLD2");
            Globals.StartingElixir2 = Globals.GlobalNumberValue("STARTING_ELIXIR2");

            Globals.SpeedUpDiamondCost1Min = Globals.GlobalNumberValue("SPEED_UP_DIAMOND_COST_1_MIN");
            Globals.SpeedUpDiamondCost1Hour = Globals.GlobalNumberValue("SPEED_UP_DIAMOND_COST_1_HOUR");
            Globals.SpeedUpDiamondCost24Hours = Globals.GlobalNumberValue("SPEED_UP_DIAMOND_COST_24_HOURS");
            Globals.SpeedUpDiamondCost1Week = Globals.GlobalNumberValue("SPEED_UP_DIAMOND_COST_1_WEEK");

            Globals.Village2SpeedUpDiamondCost1Min = Globals.GlobalNumberValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_MIN");
            Globals.Village2SpeedUpDiamondCost1Hour = Globals.GlobalNumberValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_HOUR");
            Globals.Village2SpeedUpDiamondCost24Hours = Globals.GlobalNumberValue("VILLAGE2_SPEED_UP_DIAMOND_COST_24_HOURS");
            Globals.Village2SpeedUpDiamondCost1Week = Globals.GlobalNumberValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_WEEK");

            Globals.ResourceDiamondCost100 = Globals.GlobalNumberValue("RESOURCE_DIAMOND_COST_100");
            Globals.ResourceDiamondCost1000 = Globals.GlobalNumberValue("RESOURCE_DIAMOND_COST_1000");
            Globals.ResourceDiamondCost10000 = Globals.GlobalNumberValue("RESOURCE_DIAMOND_COST_10000");
            Globals.ResourceDiamondCost100000 = Globals.GlobalNumberValue("RESOURCE_DIAMOND_COST_100000");
            Globals.ResourceDiamondCost1000000 = Globals.GlobalNumberValue("RESOURCE_DIAMOND_COST_1000000");
            Globals.ResourceDiamondCost10000000 = Globals.GlobalNumberValue("RESOURCE_DIAMOND_COST_10000000");

            Globals.DarkElixirDiamondCost1 = Globals.GlobalNumberValue("DARK_ELIXIR_DIAMOND_COST_1");
            Globals.DarkElixirDiamondCost10 = Globals.GlobalNumberValue("DARK_ELIXIR_DIAMOND_COST_10");
            Globals.DarkElixirDiamondCost100 = Globals.GlobalNumberValue("DARK_ELIXIR_DIAMOND_COST_100");
            Globals.DarkElixirDiamondCost1000 = Globals.GlobalNumberValue("DARK_ELIXIR_DIAMOND_COST_1000");
            Globals.DarkElixirDiamondCost10000 = Globals.GlobalNumberValue("DARK_ELIXIR_DIAMOND_COST_10000");
            Globals.DarkElixirDiamondCost100000 = Globals.GlobalNumberValue("DARK_ELIXIR_DIAMOND_COST_100000");

            Globals.Village2ResourceDiamondCost100 = Globals.GlobalNumberValue("VILLAGE2_RESOURCE_DIAMOND_COST_100");
            Globals.Village2ResourceDiamondCost1000 = Globals.GlobalNumberValue("VILLAGE2_RESOURCE_DIAMOND_COST_1000");
            Globals.Village2ResourceDiamondCost10000 = Globals.GlobalNumberValue("VILLAGE2_RESOURCE_DIAMOND_COST_10000");
            Globals.Village2ResourceDiamondCost100000 = Globals.GlobalNumberValue("VILLAGE2_RESOURCE_DIAMOND_COST_100000");
            Globals.Village2ResourceDiamondCost1000000 = Globals.GlobalNumberValue("VILLAGE2_RESOURCE_DIAMOND_COST_1000000");
            Globals.Village2ResourceDiamondCost10000000 = Globals.GlobalNumberValue("VILLAGE2_RESOURCE_DIAMOND_COST_10000000");

            Globals.WorkerCost2Nd = Globals.GlobalNumberValue("WORKER_COST_2ND");
            Globals.WorkerCost3Rd = Globals.GlobalNumberValue("WORKER_COST_3RD");
            Globals.WorkerCost4Th = Globals.GlobalNumberValue("WORKER_COST_4TH");
            Globals.WorkerCost5Th = Globals.GlobalNumberValue("WORKER_COST_5TH");

            Globals.TroopTrainingSpeedUpTutorialCost = Globals.GlobalNumberValue("TROOP_TRAINING_SPEED_UP_COST_TUTORIAL");
            Globals.HeroHealthSpeedUpCostMultiplier = Globals.GlobalNumberValue("HERO_HEALTH_SPEED_UP_COST_MULTIPLIER");
            Globals.SpellSpeedUpCostMultiplier = Globals.GlobalNumberValue("SPELL_SPEED_UP_COST_MULTIPLIER");
            Globals.TroopRequestSpeedUpCostMultiplier = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("TROOP_REQUEST_SPEED_UP_COST_MULTIPLIER")).NumberValue;
            Globals.TrainCancelMultiplier = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("TRAIN_CANCEL_MULTIPLIER")).NumberValue;
            Globals.BuildCancelMultiplier = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("BUILD_CANCEL_MULTIPLIER")).NumberValue;
            Globals.SpellCancelMultiplier = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("SPELL_CANCEL_MULTIPLIER")).NumberValue;
            Globals.HeroUpgradeCancelMultiplier = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("HERO_UPGRADE_CANCEL_MULTIPLIER")).NumberValue;

            Globals.AllianceTroopRequestCooldown = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("ALLIANCE_TROOP_REQUEST_COOLDOWN")).NumberValue;
            Globals.ClanMailCooldown = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("CLAN_MAIL_COOLDOWN")).NumberValue;
            Globals.ReplayShareCooldown = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("REPLAY_SHARE_COOLDOWN")).NumberValue;
            Globals.ElderKickCooldown = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("ELDER_KICK_COOLDOWN")).NumberValue;
            Globals.ChallengeCooldown = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("CHALLENGE_COOLDOWN")).NumberValue;
            Globals.ArrangeWarCooldown = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("ARRANGE_WAR_COOLDOWN")).NumberValue;

            Globals.ObstacleCountMax = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("OBSTACLE_COUNT_MAX")).NumberValue;
            Globals.ObstacleRespawnSeconds = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("OBSTACLE_RESPAWN_SECONDS")).NumberValue;

            Globals.CollectAllResourcesAtOnce = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("COLLECT_ALL_RESOURCES_AT_ONCE")).BooleanValue;

            Globals.TroopHousingV2Cost = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("TROOP_HOUSING_V2_COST")).NumberArray;
            Globals.TroopHousingV2BuildTimeSeconds = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("TROOP_HOUSING_V2_BUILD_TIME_SECONDS")).NumberArray;
            Globals.LootMultiplierByTownHallDif = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("LOOT_MULTIPLIER_BY_TH_DIFF")).NumberArray;
            Globals.BarrackReduceTrainingDivisor = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("BARRACK_REDUCE_TRAINING_DIVISOR")).NumberArray;
            Globals.DarkBarrackReduceTrainingDivisor = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("BARRACK_REDUCE_TRAINING_DIVISOR")).NumberArray;
            Globals.ClockTowerBoostMind = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("CLOCK_TOWER_BOOST_MINS")).NumberArray;

            Globals.Village2DoNotAllowClearObstacleTh = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("VILLAGE2_DO_NOT_ALLOW_CLEAR_OBSTACLE_TH")).NumberValue;
            Globals.TallGrassRespawnSeconds = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("TALLGRASS_RESPAWN_SECONDS")).NumberValue;

            Globals.AttackLength = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("ATTACK_LENGTH_SEC")).NumberValue;
            Globals.AttackPrepartionLength = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("ATTACK_PREPARATION_LENGTH_SEC")).NumberValue;
            Globals.AttackPrepartionLength2 = ((GlobalData) CSV.Tables.Get(Gamefile.Globals).GetData("ATTACK_PREPARATION_LENGTH_VILLAGE2_SEC")).NumberValue;
        }
    }
}
