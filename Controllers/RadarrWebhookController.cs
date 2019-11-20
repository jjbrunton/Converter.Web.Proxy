using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ConversionProxy.Models;
using ConversionProxy.Proxies;
using ConversionProxy.Radarr.Models;
using ConversionProxy.Services;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radarr.Models;

namespace ConversionProxy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RadarrWebhookController : ControllerBase
    {
        private readonly IDownloadProcesserService<RadarrWebhookPayload> downloadProcesserService;

        private readonly ILogger<RadarrWebhookController> logger;

        private readonly ISettingsService settingsService;
        
        public RadarrWebhookController(ILogger<RadarrWebhookController> logger, ISettingsService settingsService, IDownloadProcesserService<RadarrWebhookPayload> downloadProcesserService)
        {
            this.downloadProcesserService = downloadProcesserService;
            this.logger = logger;
            this.settingsService = settingsService;
        }

        /// <summary>
        /// Handles a webhook request from Radarr
        /// </summary>
        /// <param name="item"></param>
        /// <returns>A queue result representing the internal queue item for converting the webhook file</returns>
        /// <response code="200">Returns the internal queue item information</response>
        /// <response code="400">If the webhook call is not of type Download or Test</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Post(RadarrWebhookPayload webhookPayload)
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

        private QueueResult ProcessDownload(RadarrWebhookPayload importPayload, PerformContext context)
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

            var path = importPayload.MovieFile.Path + "/" + importPayload.MovieFile.RelativePath;

            this.logger.LogInformation($"Received download process request title: {importPayload.Movie.Title} series: {importPayload.Movie.Id} path:{path}");

            return new QueueResult()
            {
                TargetFilePath = importPayload.MovieFile.Path,
                JobId = BackgroundJob.Enqueue(() => this.downloadProcesserService.ConvertEpisode(importPayload, false, context))
            };
        }
    }
}
