using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateMetadataRequest : UnzerApiRequest
    {
        public string pluginType { get; set; }
        public string pluginVersion { get; set; }
        public string shopType { get; set; }
        public string shopVersion { get; set; }

        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => $"v1/metadata";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}
