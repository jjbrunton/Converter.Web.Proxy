namespace Sonarr.Models
{
    public class SonarrWebhookEpisodeFile
    {
        public SonarrWebhookEpisodeFile() { }

        public int Id { get; set; }
        public string RelativePath { get; set; }
        public string Path { get; set; }
        public string Quality { get; set; }
        public int QualityVersion { get; set; }
        public string ReleaseGroup { get; set; }
        public string SceneName { get; set; }
    }
}