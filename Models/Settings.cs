using System.Collections.Generic;

namespace ConversionProxy.Models
{
    public class Settings
    {
        public string ConverterLocation { get; set; }

        public string Arguments { get; set; }

        public string SonarrUrl { get; set; }

        public string SonarrApiKey { get; set; }

        public int MaxConcurrentConversions { get; set; }

        public bool NotifyPlexAutoscan { get; set; }

        public string PlexAutoscanUrl { get; set; }

        public List<PathMapping> SonarrPathMappings { get; set; }

        public string RadarrUrl { get; set; }

        public string RadarrApiKey { get; set; }

        public List<PathMapping> RadarrPathMappings { get; set; }
    }
}