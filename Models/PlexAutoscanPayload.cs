using Newtonsoft.Json;

namespace ConversionProxy.Models
{
    public class PlexAutoscanPayload
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("filepath")]
        public string Filepath { get; set; }
    }
}