using Newtonsoft.Json;

namespace Radarr.Models
{
    public class RadarrWebhookMovie
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("filePath")]
        public string FilePath { get; set; }

        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }

        [JsonProperty("folderPath")]
        public string FolderPath { get; set; }

    }
}