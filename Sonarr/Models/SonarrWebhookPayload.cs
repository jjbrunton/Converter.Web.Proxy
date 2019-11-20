using System.Collections.Generic;
using ConversionProxy.Models;
using Newtonsoft.Json;

namespace Sonarr.Models
{
    public class SonarrWebhookPayload : WebhookPayload
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }
        
        [JsonProperty("series")]
        public SonarrWebhookSeries Series { get; set; }

        [JsonProperty("episodes")]
        public List<SonarrWebhookEpisode> Episodes { get; set; }
        
        [JsonProperty("episodeFile")]
        public SonarrWebhookEpisodeFile EpisodeFile { get; set; }

        [JsonProperty("release")]
        public SonarrWebhookRelease Release { get; set; }

        [JsonProperty("isUpgrade")]
        public bool IsUpgrade { get; set; }
    }
}