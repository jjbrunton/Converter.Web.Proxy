using ConversionProxy.Models;
using Newtonsoft.Json;

namespace Converter.Web.Proxy.Models
{
    public class ManualPayload : WebhookPayload
    {
        [JsonProperty("filePath")]
        public string Filepath { get; set; }

        [JsonProperty("fileType")]
        public FileType FileType { get; set; }
    }
}