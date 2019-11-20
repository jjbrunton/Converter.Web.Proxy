using Newtonsoft.Json;

namespace Radarr.Models
{
    public class RadarrWebhookMovieFile
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("relativePath")]
        public string RelativePath { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("qualityVersion")]
        public int QualityVersion { get; set; }

        [JsonProperty("releaseGroup")]
        public string ReleaseGroup { get; set; }
        
        [JsonProperty("sceneName")]
        public string SceneName { get; set; }
    }
}