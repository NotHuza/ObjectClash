namespace CR.Servers.CoC.Logic
{
    using Newtonsoft.Json.Linq;

    internal class Component
    {
        internal GameObject Parent;

        public Component(GameObject GameObject)
        {
            this.Parent = GameObject;
        }

        internal virtual int Checksum
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

        internal virtual void FastForwardTime(int Secs)
        {
        }

        internal virtual void Load(JToken Json)
        {
        }

        internal virtual void Save(JObject Json)
        {
        }

        internal virtual void Tick()
        {
        }
    }
}