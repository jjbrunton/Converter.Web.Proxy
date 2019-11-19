using System.Collections.Generic;

namespace ConversionProxy.Models
{
    public class Settings
    {
        public string ConverterLocation { get; set; }

        public string Arguments { get; set; }

        public string SonarrUrl { get; set; }

        public string SonarrApiKey { get; set; }

        public List<SonarrPathMapping> SonarrPathMappings { get; set; }
    }
}