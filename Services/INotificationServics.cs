using System.Threading.Tasks;
using ConversionProxy.Models;
using Radarr.Models;

namespace ConversionProxy.Services
{
    public interface INotificationService<T> where T : WebhookPayload
    {
         Task NotifyService(T importPayload, bool isTest, Hangfire.Server.PerformContext performContext);
    }
}