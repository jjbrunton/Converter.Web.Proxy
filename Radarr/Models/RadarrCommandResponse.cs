using System;
using Newtonsoft.Json;

namespace ConversionProxy.Radarr.Models
{
    public class RadarrCommandResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("startedOn")]
        public DateTimeOffset StartedOn { get; set; }

        [JsonProperty("stateChangeTime")]
        public DateTimeOffset StateChangeTime { get; set; }

        [JsonProperty("sendUpdatesToClient")]
        public bool SendUpdatesToClient { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }
}