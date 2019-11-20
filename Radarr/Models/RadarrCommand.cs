using Newtonsoft.Json;

namespace ConversionProxy.Radarr.Models
{
    public class RadarrCommand
    {
        [JsonProperty("movieId")]
        public int? MovieId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}