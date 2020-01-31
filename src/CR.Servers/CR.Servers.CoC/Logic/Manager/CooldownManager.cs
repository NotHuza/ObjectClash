namespace CR.Servers.CoC.Logic.Manager
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using Newtonsoft.Json.Linq;

    internal class CooldownManager
    {
        internal List<Cooldown> Cooldowns;

        internal Level Level;

        public CooldownManager(Level Level)
        {
            this.Level = Level;
            this.Cooldowns = new List<Cooldown>();
        }

        internal void AddCooldown(int Cooldown, int Target)
        {
            this.Cooldowns.Add(new Cooldown(this.Level, Cooldown, Target));
        }

        internal void FastForwardTime(int Seconds)
        {
            for (int i = 0; i < this.Cooldowns.Count; i++)
            {
             this.Cooldowns[i].FastForwardTime(Seconds);   
            }
        }

        internal int GetCooldownSeconds(int TargetID)
        {
            Cooldown Cooldown = this.Cooldowns.Find(cooldown => cooldown.Target == TargetID);

            if (Cooldown != null)
            {
                return Cooldown.CooldownTimer.GetRemainingSeconds(this.Level.Player.LastTick);
            }

            return 0;
        }

        internal void Load(JToken Token)
        {
            JArray Cooldowns;
            if (JsonHelper.GetJsonArray(Token, "cooldowns", out Cooldowns))
            {
                foreach (JToken Object in Cooldowns)
                {
                    Cooldown Cooldown = new Cooldown(this.Level);
                    Cooldown.Load(Object);
                    this.Cooldowns.Add(Cooldown);
                }
            }
        }

        internal void Save(JObject Json)
        {
            JArray Cooldowns = new JArray();

            for (int t = 0; t < this.Cooldowns.Count; t++)
            {

                Cooldowns.Add(this.Cooldowns[t].Save());
                
            }

            Json.Add("cooldowns", Cooldowns);
        }

        internal void Tick()
        {
            for (int i = 0; i < this.Cooldowns.Count; i++)
            {
                if (this.Cooldowns[i].CooldownTimer.GetRemainingSeconds(this.Level.Player.LastTick) <= 0)
                {
                    this.Cooldowns.RemoveAt(i--);
                }
            }
        }
    }
}