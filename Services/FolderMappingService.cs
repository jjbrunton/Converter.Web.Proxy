using System.IO;
using ConversionProxy.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace ConversionProxy.Services
{
    public class FolderMappingService : IFolderMappingService
    {
        private readonly ISettingsService settingsService;
        private readonly ILogger<FolderMappingService> logger;

        public FolderMappingService(ISettingsService settingsService, ILogger<FolderMappingService> logger)
        {
            this.settingsService = settingsService;
            this.logger = logger;
        }
        
        public string ReplacePathWithMappings(string path)
        {
            this.logger.LogInformation($"Looking for mappings in {path}");
            foreach (var mapping in this.settingsService.Settings.SonarrPathMappings)
            {
                if (path.Contains(mapping.Remote))
                {
                    this.logger.LogInformation($"Found mapping from {mapping.Remote} to {mapping.Local}");
                    return path.Replace(mapping.Remote, mapping.Local);
                }
            }

            return path;
        }
    }
}