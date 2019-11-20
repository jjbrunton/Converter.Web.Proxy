using Newtonsoft.Json;

namespace Sonarr.Models 
{
    public class SonarrWebhookSeries
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("tvdbId")]
        public int TvdbId { get; set; }
    }
}