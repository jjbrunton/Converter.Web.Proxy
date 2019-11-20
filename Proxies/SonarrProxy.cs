using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Sonarr.Models;
using Refit;

namespace ConversionProxy.Proxies
{
    public interface ISonarrProxy
    {
        [Post("/api/command")]
        Task<SonarrCommandResponse> ExecuteCommand([Header("X-Api-Key")] string apiKey, [Body]SonarrCommand command);
    }
}