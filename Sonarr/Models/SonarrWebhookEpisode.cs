using System;
using Newtonsoft.Json;

namespace Sonarr.Models
{
    public class SonarrWebhookEpisode
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("episodeNumber")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("seasonNumber")]
        public int SeasonNumber { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("airDate")]
        public string AirDate { get; set; }

        [JsonProperty("airDateUtc")]
        public DateTime? AirDateUtc { get; set; }

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