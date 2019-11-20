using System.Collections.Generic;
using ConversionProxy.Models;
using Newtonsoft.Json;

namespace Radarr.Models
{
    public class RadarrWebhookPayload : WebhookPayload
    {
        [JsonProperty("remoteMovie")]
        public RadarrWebhookRemoteMovie RemoteMovie { get; set; }

        [JsonProperty("movieFile")]
        public RadarrWebhookMovieFile MovieFile { get; set; }

        [JsonProperty("release")]
        public RadarrWebhookRelease Release { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("movie")]
        public RadarrWebhookMovie Movie { get; set; }

        [JsonProperty("isUpgrade")]
        public bool IsUpgrade { get; set; }
    }
}