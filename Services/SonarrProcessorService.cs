using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Proxies;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radarr.Models;
using Sonarr.Models;

namespace ConversionProxy.Services
{
    public class SonarrProcessorService : IDownloadProcesserService<SonarrWebhookPayload>
    {
        private readonly ILogger<RadarrService> logger;
        private readonly ISettingsService settingsService;
        private readonly IFolderMappingService folderMappingService;
        private readonly INotificationService<SonarrWebhookPayload> sonarrService;

        private readonly IPlexAutoscanProxy plexAutoscanProxy;

        public SonarrProcessorService(ILogger<RadarrService> logger, ISettingsService settingsService, IFolderMappingService folderMappingService, INotificationService<SonarrWebhookPayload> sonarrService, IPlexAutoscanProxy plexAutoscanProxy)
        {
            this.plexAutoscanProxy = plexAutoscanProxy;
            this.sonarrService = sonarrService;
            this.folderMappingService = folderMappingService;
            this.settingsService = settingsService;
            this.logger = logger;
        }

        [AutomaticRetry(Attempts = 3)]
        public async Task ConvertEpisode(SonarrWebhookPayload importPayload, bool isTest, PerformContext performContext)
        {
            this.logger.LogInformation($"Conversion beginning");
            performContext.WriteLine($"Conversion beginning");
            var path = isTest ? "test.mkv" : importPayload.Series.Path + "/" + importPayload.EpisodeFile.RelativePath;
            try
            {
                using (Process converter = new Process())
                {
                    converter.StartInfo.UseShellExecute = false;
                    converter.StartInfo.FileName = this.settingsService.Settings.ConverterLocation;
                    converter.StartInfo.Arguments = string.Format(this.settingsService.Settings.Arguments, this.folderMappingService.ReplacePathWithMappings(path, this.settingsService.Settings.SonarrPathMappings));
                    converter.Start();
                    converter.WaitForExit();
                    this.logger.LogInformation("Conversion completed");
                    performContext.WriteLine($"Conversion completed");
                    await this.sonarrService.NotifyService(importPayload, isTest, performContext);
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e.Message);
            }

            if (this.settingsService.Settings.NotifyPlexAutoscan)
            {
                this.logger.LogInformation("Notifying Plex Autoscan");
                var request = new PlexAutoscanPayload() {
                    EventType = "Manual",
                    Filepath = importPayload.Series.Path
                };
                performContext.WriteLine($"Notifying Plex Autoscan with payload: {JsonConvert.SerializeObject(request)}");
                await this.plexAutoscanProxy.Notify(request);
            }
        }

    }
}