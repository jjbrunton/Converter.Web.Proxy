using Newtonsoft.Json;

namespace Radarr.Models
{
    public class RadarrWebhookRelease
    {
        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("qualityVersion")]
        public int QualityVersion { get; set; }

        [JsonProperty("releaseGroup")]
        public string ReleaseGroup { get; set; }

        [JsonProperty("releaseTitle")]
        public string ReleaseTitle { get; set; }

        [JsonProperty("indexer")]
        public string Indexer { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }
    }
}