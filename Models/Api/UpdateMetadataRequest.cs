using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class UpdateMetadataRequest : UnzerApiRequest
{
    [JsonIgnore]
    public string metadataId { get; set; }

    public string pluginType { get; set; }
    public string pluginVersion { get; set; }
    public string shopType { get; set; }
    public string shopVersion { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => $"v1/metadata/{metadataId}";

    [JsonIgnore]
    public override string Method => HttpMethods.Put;
}
