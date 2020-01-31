namespace CR.Servers.CoC.Logic
{
    using Newtonsoft.Json;

    internal class Functions
    {
        [JsonProperty("name")] internal string Name = string.Empty;

        [JsonProperty("parameters")] internal string[] Parameters;
    }
}