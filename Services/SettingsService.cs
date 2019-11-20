using System.IO;
using ConversionProxy.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConversionProxy.Services
{
    public class SettingsService : ISettingsService
    {
        private Settings settings;

        public SettingsService(IHostingEnvironment env, ILogger<SettingsService> logger)
        {
            logger.LogInformation("Loading settings");
            var configPath = env.ContentRootPath + "/config/settings.json";
            if (File.Exists(configPath))
            {
                string fileInput = File.ReadAllText(configPath);
                this.settings = JsonConvert.DeserializeObject<Settings>(fileInput);
                logger.LogInformation($"Settings loaded: {JsonConvert.SerializeObject(this.settings)}");
            } else {
                logger.LogError($"Config not found at {configPath}");
            }
        }

        public Settings Settings => this.settings;
    }
}