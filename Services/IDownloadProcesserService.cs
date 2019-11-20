using System.Threading.Tasks;
using ConversionProxy.Models;
using Hangfire.Server;

namespace ConversionProxy.Services
{
    public interface IDownloadProcesserService<T> where T : WebhookPayload 
    {
         Task ConvertEpisode(T importPayload, bool isTest, PerformContext context);
    }
}