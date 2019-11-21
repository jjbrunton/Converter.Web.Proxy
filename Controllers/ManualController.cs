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
using Converter.Web.Proxy.Models;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Radarr.Models;

namespace ConversionProxy.Controllers {
    [ApiController]
    [Route ("[controller]")]
    public class ManualController : ControllerBase {
        private readonly ILogger<ManualController> logger;

        private readonly ISettingsService settingsService;

        private readonly IDownloadProcesserService<ManualPayload> downloadProcesserService;

        public ManualController (ILogger<ManualController> logger, ISettingsService settingsService, IDownloadProcesserService<ManualPayload> downloadProcesserService) {
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
        [ProducesResponseType (StatusCodes.Status200OK)]
        [ProducesResponseType (StatusCodes.Status400BadRequest)]
        public IActionResult Post (ManualPayload webhookPayload) {
            logger.LogTrace (new EventId (), null, webhookPayload.ToString (), null);
            logger.LogInformation (JsonConvert.SerializeObject (webhookPayload));
            if (!System.IO.File.Exists(webhookPayload.Filepath))
            {
                return this.BadRequest("File does not exist");
            }

            return this.Ok(new QueueResult()
            {
                TargetFilePath = webhookPayload.Filepath,
                JobId = BackgroundJob.Enqueue(() => this.downloadProcesserService.ConvertEpisode(webhookPayload, false, null))
            });
        }
    }
}