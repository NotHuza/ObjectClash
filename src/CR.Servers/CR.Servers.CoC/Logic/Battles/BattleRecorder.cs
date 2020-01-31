using CR.Servers.Core.Consoles.Colorful;

namespace CR.Servers.CoC.Logic.Battles
{
    using CR.Servers.CoC.Extensions;
    using Newtonsoft.Json.Linq;

    internal class BattleRecorder
    {
        internal JObject Attacker;
        internal Battle Battle;
        internal JArray Commands;
        internal JObject Defender;

        internal int EndTick;
        internal JObject Globals;

        internal JObject Level;
        internal JToken PreparationSkip;
        internal int Timestamp;

        internal BattleRecorder(Battle battle)
        {
            this.Battle = battle;

            this.Level = battle.Defender.Save();
            this.Attacker = battle.Attacker.Player.Save();
            this.Defender = battle.Defender.Player.Save();
            this.Globals = JObject.Parse("{\"Globals\":{\"GiftPackExtension\":\"birthday\",\"DuelLootLimitCooldownInMinutes\":1320,\"DuelBonusLimitWinsPerDay\":3,\"DuelBonusPercentWin\":100,\"DuelBonusPercentLose\":0,\"DuelBonusPercentDraw\":0,\"DuelBonusMaxDiamondCostPercent\":100},\"KillSwitches\":{\"BattleWaitForProjectileDestruction\":true,\"BattleWaitForDieDamage\":true},\"Village2\":{\"StrengthRangeForScore\":[{\"Percentage\":60,\"Milestone\":0},{\"Percentage\":80,\"Milestone\":200},{\"Percentage\":100,\"Milestone\":400},{\"Percentage\":120,\"Milestone\":600},{\"Percentage\":140,\"Milestone\":800},{\"Percentage\":160,\"Milestone\":1000},{\"Percentage\":180,\"Milestone\":1200},{\"Percentage\":200,\"Milestone\":1400},{\"Percentage\":400,\"Milestone\":1600},{\"Percentage\":600,\"Milestone\":1800},{\"Percentage\":1000,\"Milestone\":2000}],\"TownHallMaxLevel\":8,\"ScoreChangeForLosing\":[{\"Percentage\":0,\"Milestone\":0},{\"Percentage\":30,\"Milestone\":400},{\"Percentage\":55,\"Milestone\":800},{\"Percentage\":70,\"Milestone\":1200},{\"Percentage\":85,\"Milestone\":1600},{\"Percentage\":95,\"Milestone\":2000},{\"Percentage\":100,\"Milestone\":2400}]},\"Village1\":{\"SpecialObstacle\":\"Halloween2017\"}}");

            if (battle.Duel)
            {
                this.Level.Add("direct2", true);
            }

            this.Commands = new JArray();
            this.Timestamp = TimeUtils.UnixUtcNow;
        }

        internal JObject Save()
        {
            JObject json = new JObject();
            
            json.Add("level", this.Level);
            json.Add("attacker", this.Attacker);
            json.Add("defender", this.Defender);
            json.Add("cmd", this.Commands);
            json.Add("end_tick", this.EndTick);
            json.Add("timestamp", this.Timestamp);
            json.Add("globals", this.Globals);
            json.Add("calendar", new JObject());
            /*
            if (this.PreparationSkip != null)
            {
                json.Add("prep_skip", this.PreparationSkip);
            }
            */
            return json;
        }
    }
}