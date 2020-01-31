namespace CR.Servers.CoC.Logic
{
    using System;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Helper;
    using Newtonsoft.Json.Linq;

    internal class Cooldown
    {
        internal Timer CooldownTimer;
        internal Level Level;

        internal int Target;

        public Cooldown(Level Level)
        {
            this.Level = Level;
        }

        public Cooldown(Level Level, int Cooldown, int Target)
        {
            this.Level = Level;

            this.CooldownTimer = new Timer();
            this.CooldownTimer.StartTimer(DateTime.Now, Cooldown);

            this.Target = Target;
        }

        internal int CooldownSeconds
        {
            get
            {
                int Seconds = this.CooldownTimer.GetRemainingSeconds(this.Level.Player.LastTick);
                long var = ((-2004318071L * Seconds) >> 32) + Seconds;
                return (int) ((var >> 3) + (var >> 31));
            }
        }

        internal void Load(JToken Token)
        {
            int Cooldown;
            int CooldownEnd;
            if (JsonHelper.GetJsonNumber(Token, "cooldown", out Cooldown) && JsonHelper.GetJsonNumber(Token, "cooldown_end", out CooldownEnd))
            {
                if (Cooldown > -1)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Level.Player.LastTick);
                    int duration = CooldownEnd - startTime;
                    if (duration < 0)
                    {
                        duration = 0;
                    }
                    //ConstructionTime = Math.Min(ConstructionTime, Data.GetBuildTime(this.UpgradeLevel + 1));

                    this.CooldownTimer = new Timer();
                    this.CooldownTimer.StartTimer(this.Level.Player.LastTick, duration);
                }
            }

            if (!JsonHelper.GetJsonNumber(Token, "target", out this.Target))
            {
                Logging.Error(this.GetType(), "Load() - Target was not found!");
            }
        }

        internal JObject Save()
        {
            JObject Json = new JObject();

            if (this.CooldownTimer != null)
            {
                Json.Add("cooldown", this.CooldownSeconds);
                Json.Add("cooldown_end", this.CooldownTimer.EndTime);
                Json.Add("target", this.Target);
            }

            return Json;
        }

        internal void FastForwardTime(int Seconds)
        {
            if (this.CooldownTimer != null)
            {
                this.CooldownTimer.FastForward(Seconds);
            }
        }
    }
}