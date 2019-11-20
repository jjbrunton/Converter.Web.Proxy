using System.Collections.Generic;
using ConversionProxy.Models;

namespace ConversionProxy.Services
{
    public interface IFolderMappingService
    {
        string ReplacePathWithMappings(string path, List<PathMapping> pathMappings);
    }
}