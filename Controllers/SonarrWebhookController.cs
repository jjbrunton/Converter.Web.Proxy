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

        private readonly ILogger<SonarrWebhookController> _logger;
        private readonly ISettingsService settingsService;
        private readonly ISonarrProxy sonarrProxy;

        public SonarrWebhookController(ILogger<SonarrWebhookController> logger, ISettingsService settingsService, ISonarrProxy sonarrProxy)
        {
            _logger = logger;
            this.settingsService = settingsService;
            this.sonarrProxy = sonarrProxy;
        }

        [HttpPost]
        public IActionResult Post(SonarrWebhookPayload webhookPayload)
        {
            _logger.LogTrace(new EventId(), null, webhookPayload.ToString(), null);
            _logger.LogInformation(JsonConvert.SerializeObject(webhookPayload));
            switch (webhookPayload.EventType)
            {
                case "Download":
                case "Test":
                    return this.Ok(this.ProcessDownload(webhookPayload));
                default:
                    this._logger.LogInformation($"Request was not of type Download or Test");
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

            this._logger.LogInformation($"Received download process request title: {importPayload.Series.Title} series: {importPayload.Series.Id} path:{importPayload.EpisodeFile.Path}");

            return new QueueResult()
            {
                TargetFilePath = importPayload.EpisodeFile.Path,
                JobId = BackgroundJob.Enqueue(() => this.ConvertEpisode(importPayload, false))
            };
        }

        public async Task NotifySonarr(SonarrWebhookPayload importPayload, bool isTest)
        {
            var path = isTest ? "test.mkv" : importPayload.Series.Path + "/" + importPayload.EpisodeFile.RelativePath;
            this._logger.LogInformation($"Informing Sonarr of conversion result path: {path}");
            var response = await this.sonarrProxy.ExecuteCommand(this.settingsService.Settings.SonarrApiKey, new SonarrCommand() {
                Name = "RescanSeries",
                SeriesId = isTest ? 10 : importPayload.Series.Id
            });

            this._logger.LogInformation($"Sonarr response: {response.State}");
        }

        public string ReplacePathWithMappings(string path)
        {
            this._logger.LogInformation($"Looking for mappings in {path}");
            foreach (var mapping in this.settingsService.Settings.SonarrPathMappings)
            {
                if (path.Contains(mapping.Remote))
                {
                    this._logger.LogInformation($"Found mapping from {mapping.Remote} to {mapping.Local}");
                    return path.Replace(mapping.Remote, mapping.Local);
                }
            }

            return path;
        }

        public async Task ConvertEpisode(SonarrWebhookPayload importPayload, bool isTest)
        {
            this._logger.LogInformation($"Conversion beginning");
            var path = isTest ? "test.mkv" : importPayload.Series.Path + "/" + importPayload.EpisodeFile.RelativePath;
            try
            {
                using (Process converter = new Process())
                {
                    converter.StartInfo.UseShellExecute = false;
                    converter.StartInfo.FileName = this.settingsService.Settings.ConverterLocation;
                    converter.StartInfo.Arguments = string.Format(this.settingsService.Settings.Arguments, this.ReplacePathWithMappings(path));
                    //converter.StartInfo.CreateNoWindow = true;
                    converter.Start();
                    converter.WaitForExit();
                    this._logger.LogInformation("Conversion completed");
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
