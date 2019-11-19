using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Proxies;
using ConversionProxy.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sonarr.Models;

namespace ConversionProxy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SonarrWebhookController : ControllerBase
    {

        private readonly ILogger<SonarrWebhookController> logger;
        private readonly ISettingsService settingsService;
        private readonly ISonarrProxy sonarrProxy;
        private readonly IFolderMappingService folderMappingService;

        public SonarrWebhookController(ILogger<SonarrWebhookController> logger, ISettingsService settingsService, ISonarrProxy sonarrProxy, IFolderMappingService folderMappingService)
        {
            this.logger = logger;
            this.settingsService = settingsService;
            this.sonarrProxy = sonarrProxy;
            this.folderMappingService = folderMappingService;
        }

        [HttpPost]
        public IActionResult Post(SonarrWebhookPayload webhookPayload)
        {
            logger.LogTrace(new EventId(), null, webhookPayload.ToString(), null);
            logger.LogInformation(JsonConvert.SerializeObject(webhookPayload));
            switch (webhookPayload.EventType)
            {
                case "Download":
                case "Test":
                    return this.Ok(this.ProcessDownload(webhookPayload));
                default:
                    this.logger.LogInformation($"Request was not of type Download or Test");
                    return StatusCode(400);
            }
        }

        private QueueResult ProcessDownload(SonarrWebhookPayload importPayload)
        {
            if (importPayload.EventType == "Test")
            {
                return new QueueResult()
                {
                    TargetFilePath = "/test/path",
                    JobId = BackgroundJob.Enqueue(() => this.ConvertEpisode(importPayload, true))
                };
            }

            this.logger.LogInformation($"Received download process request title: {importPayload.Series.Title} series: {importPayload.Series.Id} path:{importPayload.EpisodeFile.Path}");

            return new QueueResult()
            {
                TargetFilePath = importPayload.EpisodeFile.Path,
                JobId = BackgroundJob.Enqueue(() => this.ConvertEpisode(importPayload, false))
            };
        }

        public async Task NotifySonarr(SonarrWebhookPayload importPayload, bool isTest)
        {
            var path = isTest ? "test.mkv" : importPayload.Series.Path + "/" + importPayload.EpisodeFile.RelativePath;
            this.logger.LogInformation($"Informing Sonarr of conversion result path: {path}");
            var response = await this.sonarrProxy.ExecuteCommand(this.settingsService.Settings.SonarrApiKey, new SonarrCommand() {
                Name = "RescanSeries",
                SeriesId = isTest ? 10 : importPayload.Series.Id
            });

            this.logger.LogInformation($"Sonarr response: {response.State}");
        }

        public async Task ConvertEpisode(SonarrWebhookPayload importPayload, bool isTest)
        {
            this.logger.LogInformation($"Conversion beginning");
            var path = isTest ? "test.mkv" : importPayload.Series.Path + "/" + importPayload.EpisodeFile.RelativePath;
            try
            {
                using (Process converter = new Process())
                {
                    converter.StartInfo.UseShellExecute = false;
                    converter.StartInfo.FileName = this.settingsService.Settings.ConverterLocation;
                    converter.StartInfo.Arguments = string.Format(this.settingsService.Settings.Arguments, this.folderMappingService.ReplacePathWithMappings(path));
                    converter.Start();
                    converter.WaitForExit();
                    this.logger.LogInformation("Conversion completed");
                    await this.NotifySonarr(importPayload, isTest);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Ending");
        }
    }
}
