namespace CR.Servers.CoC.Logic
{
    using Facebook;
    using Newtonsoft.Json;

    internal class FacebookApi
    {
        internal const string ApplicationId = "319595285114483";
        internal const string ApplicationSecret = "e247977dc751f310c8861f08a48917c8";
        internal const string ApplicationVersion = "2.11";

        internal FacebookClient FBClient;

        [JsonProperty("fb_id")] internal string Identifier;
        internal Player Player;
        [JsonProperty("fb_token")] internal string Token;

        internal FacebookApi()
        {
        }

        internal FacebookApi(Player Player)
        {
            this.Player = Player;

            if (this.Filled)
            {
                this.Connect();
            }
        }

        internal bool Connected
        {
            get
            {
                return this.Filled && this.FBClient != null;
            }
        }

        internal bool Filled
        {
            get
            {
                return !string.IsNullOrEmpty(this.Identifier) && !string.IsNullOrEmpty(this.Token);
            }
        }

        internal void Connect()
        {
            this.FBClient = new FacebookClient(this.Token)
            {
                AppId = FacebookApi.ApplicationId,
                AppSecret = FacebookApi.ApplicationSecret,
                Version = FacebookApi.ApplicationVersion
            };
        }

        internal object Get(string Path, bool IncludeIdentifier = true)
        {
            return this.Connected ? this.FBClient.Get("https://graph.facebook.com/v" + FacebookApi.ApplicationVersion + "/" + (IncludeIdentifier ? this.Identifier + '/' + Path : Path)) : null;
        }
    }
}