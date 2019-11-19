namespace Sonarr.Models 
{
    public class SonarrWebhookSeries
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public int TvdbId { get; set; }
    }
}