using System.Threading.Tasks;
using ConversionProxy.Proxies;
using ConversionProxy.Radarr.Models;
using ConversionProxy.Sonarr.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Radarr.Models;
using Sonarr.Models;

namespace ConversionProxy.Services
{

    public class SonarrService : INotificationService<SonarrWebhookPayload>
    {
        private readonly ISonarrProxy sonarrProxy;
        private readonly ISettingsService settingsService;
        private readonly ILogger<SonarrService> logger;

        public SonarrService(ILogger<SonarrService> logger, ISonarrProxy sonarrProxy, ISettingsService settingsService)
        {
            this.logger = logger;
            this.settingsService = settingsService;
            this.sonarrProxy = sonarrProxy;
        }

        public async Task NotifyService(SonarrWebhookPayload importPayload, bool isTest, PerformContext performContext)
        {
            var path = isTest ? "test.mkv" : importPayload.Series.Path + "/" + importPayload.EpisodeFile.RelativePath;
            this.logger.LogInformation($"Informing Sonarr of conversion result path: {path}");
            performContext.WriteLine($"Informing Sonarr of conversion result path: {path}");

            var response = await this.sonarrProxy.ExecuteCommand(this.settingsService.Settings.SonarrApiKey, new SonarrCommand() {
                Name = "RescanSeries",
                SeriesId = isTest ? 10 : importPayload.Series.Id
            });

            performContext.WriteLine($"Sonarr response: {response.State}");
            this.logger.LogInformation($"Sonarr response: {response.State}");
        }
    }
}