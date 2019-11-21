using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Proxies;
using ConversionProxy.Services;
using Converter.Web.Proxy.Models;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Converter.Web.Proxy.Services {
    public class ManualProcessorService : IDownloadProcesserService<ManualPayload> {
        private readonly ILogger<ManualProcessorService> logger;
        private readonly ISettingsService settingsService;

        private readonly IPlexAutoscanProxy plexAutoscanProxy;

        public ManualProcessorService (ILogger<ManualProcessorService> logger, ISettingsService settingsService, IPlexAutoscanProxy plexAutoscanProxy) {
            this.plexAutoscanProxy = plexAutoscanProxy;
            this.settingsService = settingsService;
            this.logger = logger;
        }

        public async Task ConvertEpisode (ManualPayload importPayload, bool isTest, PerformContext performContext) {
            this.logger.LogInformation ($"Conversion beginning");
            performContext.WriteLine ($"Conversion beginning");

            try {
                using (Process converter = new Process ()) {
                    converter.StartInfo.UseShellExecute = false;
                    converter.StartInfo.FileName = this.settingsService.Settings.ConverterLocation;
                    converter.StartInfo.Arguments = string.Format (this.settingsService.Settings.Arguments, importPayload.Filepath);
                    converter.Start ();
                    converter.WaitForExit ();
                    this.logger.LogInformation ("Conversion completed");
                    performContext.WriteLine ($"Conversion completed");
                }
            } catch (Exception e) {
                this.logger.LogError (e.Message);
            }

            if (this.settingsService.Settings.NotifyPlexAutoscan) {
                this.logger.LogInformation ("Notifying Plex Autoscan");
                var request = new PlexAutoscanPayload () {
                    EventType = "Manual",
                    Filepath = importPayload.Filepath
                };
                performContext.WriteLine ($"Notifying Plex Autoscan with payload: {JsonConvert.SerializeObject(request)}");
                await this.plexAutoscanProxy.Notify (request);
            }
        }
    }
}