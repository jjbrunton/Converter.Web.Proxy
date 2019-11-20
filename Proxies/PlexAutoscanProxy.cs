using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Radarr.Models;
using Refit;

namespace ConversionProxy.Proxies
{
    public interface IPlexAutoscanProxy
    {
        [Headers("Content-Type: application/json")]
        [Post("")]
        Task Notify([Body] PlexAutoscanPayload payload);
    }
}