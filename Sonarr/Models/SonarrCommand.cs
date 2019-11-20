using Newtonsoft.Json;

namespace ConversionProxy.Sonarr.Models
{
    public class SonarrCommand
    {
        [JsonProperty("seriesId")]
        public int? SeriesId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}