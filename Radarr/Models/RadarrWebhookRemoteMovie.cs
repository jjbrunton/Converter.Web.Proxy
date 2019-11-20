using System;
using Newtonsoft.Json;

namespace Radarr.Models
{
    public class RadarrWebhookRemoteMovie
    {
        [JsonProperty("tmdbId")]
        public int TmdbId { get; set; }

        [JsonProperty("imdbId")]
        public string ImdbId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
        
        [JsonProperty("year")]
        public int Year { get; set; }
    }
}