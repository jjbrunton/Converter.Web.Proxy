using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Radarr.Models;
using Refit;

namespace ConversionProxy.Proxies
{
    public interface IRadarrProxy
    {
        [Post("/api/command")]
        Task<RadarrCommandResponse> ExecuteCommand([Header("X-Api-Key")] string apiKey, [Body]RadarrCommand command);
    }
}