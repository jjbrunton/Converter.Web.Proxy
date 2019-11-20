using System.Threading.Tasks;
using ConversionProxy.Proxies;
using ConversionProxy.Radarr.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Radarr.Models;

namespace ConversionProxy.Services
{

    public class RadarrService : INotificationService<RadarrWebhookPayload>
    {
        private readonly IRadarrProxy radarrProxy;
        private readonly ISettingsService settingsService;
        private readonly ILogger<RadarrService> logger;

        public RadarrService(ILogger<RadarrService> logger, IRadarrProxy radarrProxy, ISettingsService settingsService)
        {
            this.logger = logger;
            this.settingsService = settingsService;
            this.radarrProxy = radarrProxy;
        }

        public async Task NotifyService(RadarrWebhookPayload importPayload, bool isTest, PerformContext performContext)
        {
            var path = isTest ? "test.mkv" : importPayload.Movie.FolderPath + "/" + importPayload.Movie.FilePath;
            this.logger.LogInformation($"Informing Radarr of conversion result path: {path}");
            performContext.WriteLine($"Informing Radarr of conversion result path: {path}");
            var response = await this.radarrProxy.ExecuteCommand(this.settingsService.Settings.RadarrApiKey, new RadarrCommand()
            {
                Name = "RescanMovie",
                MovieId = isTest ? 10 : importPayload.Movie.Id
            });

            this.logger.LogInformation($"Radarr response: {response.State}");
            performContext.WriteLine($"Radarr response: {response.State}");
        }
    }
}