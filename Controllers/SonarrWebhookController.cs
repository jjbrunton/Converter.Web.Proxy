using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Proxies;
using ConversionProxy.Services;
using ConversionProxy.Sonarr.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
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
        private readonly IDownloadProcesserService<SonarrWebhookPayload> downloadProcesserService;

        public SonarrWebhookController(ILogger<SonarrWebhookController> logger, ISettingsService settingsService, IDownloadProcesserService<SonarrWebhookPayload> downloadProcesserService)
        {
            this.downloadProcesserService = downloadProcesserService;
            this.logger = logger;
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Handles a webhook request from Sonarr
        /// </summary>
        /// <param name="item"></param>
        /// <returns>A queue result representing the internal queue item for converting the webhook file</returns>
        /// <response code="200">Returns the internal queue item information</response>
        /// <response code="400">If the webhook call is not of type Download or Test</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post(SonarrWebhookPayload webhookPayload)
        {
            logger.LogTrace(new EventId(), null, webhookPayload.ToString(), null);
            logger.LogInformation(JsonConvert.SerializeObject(webhookPayload));
            switch (webhookPayload.EventType)
            {
                case "Download":
                case "Test":
                    return this.Ok(this.ProcessDownload(webhookPayload, null));
                default:
                    this.logger.LogInformation($"Request was not of type Download or Test");
                    return StatusCode(400);
            }
        }

        private QueueResult ProcessDownload(SonarrWebhookPayload importPayload, PerformContext context)
        {
            context.WriteLine(JsonConvert.SerializeObject(importPayload));
            if (importPayload.EventType == "Test")
            {
                return new QueueResult()
                {
                    TargetFilePath = "/test/path",
                    JobId = BackgroundJob.Enqueue(() => this.downloadProcesserService.ConvertEpisode(importPayload, true, context))
                };
            }
            context.WriteLine($"Received download process request title: {importPayload.Series.Title} series: {importPayload.Series.Id} path:{importPayload.EpisodeFile.Path}");
            this.logger.LogInformation($"Received download process request title: {importPayload.Series.Title} series: {importPayload.Series.Id} path:{importPayload.EpisodeFile.Path}");

            return new QueueResult()
            {
                TargetFilePath = importPayload.EpisodeFile.Path,
                JobId = BackgroundJob.Enqueue(() => this.downloadProcesserService.ConvertEpisode(importPayload, false, context))
            };
        }
    }
}
