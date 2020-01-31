namespace CR.Servers.CoC.Logic
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    internal class Calendar
    {
        [JsonProperty("events")] internal List<Event> Events = new List<Event>();
    }
}