using System.IO;
using ConversionProxy.Models;
using Newtonsoft.Json;

namespace ConversionProxy.Services
{
    public class SettingsService : ISettingsService
    {
        private Settings settings;

        public SettingsService()
        {
            if (File.Exists("config/settings.json"))
            {
                string fileInput = File.ReadAllText("config/settings.json");
                this.settings = JsonConvert.DeserializeObject<Settings>(fileInput);
            }
        }

        public Settings Settings => this.settings;
    }
}