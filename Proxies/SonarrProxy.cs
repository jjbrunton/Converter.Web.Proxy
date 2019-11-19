using System.Threading.Tasks;
using ConversionProxy.Models;
using Refit;

namespace ConversionProxy.Proxies
{
    [Headers("Header-A: 1")]
    public interface ISonarrProxy
    {
        [Post("/api/command")]
        Task<SonarrCommandResponse> ExecuteCommand([Header("X-Api-Key")] string apiKey, [Body]SonarrCommand command);
    }
}