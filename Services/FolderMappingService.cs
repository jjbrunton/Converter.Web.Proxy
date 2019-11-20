using ConversionProxy.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ConversionProxy.Services
{
    public class FolderMappingService : IFolderMappingService
    {
        private readonly ILogger<FolderMappingService> logger;

        public FolderMappingService(ILogger<FolderMappingService> logger)
        {
            this.logger = logger;
        }
        
        public string ReplacePathWithMappings(string path, List<PathMapping> pathMappings)
        {
            this.logger.LogInformation($"Looking for mappings in {path}");
            foreach (var mapping in pathMappings)
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