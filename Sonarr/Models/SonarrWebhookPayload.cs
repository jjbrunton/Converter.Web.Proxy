using System.Collections.Generic;

namespace Sonarr.Models
{
    public class SonarrWebhookPayload
    {
        public string EventType { get; set; }
        
        public SonarrWebhookSeries Series { get; set; }

        public List<SonarrWebhookEpisode> Episodes { get; set; }
        
        public SonarrWebhookEpisodeFile EpisodeFile { get; set; }

        public SonarrWebhookRelease Release { get; set; }

        public bool IsUpgrade { get; set; }
    }
}