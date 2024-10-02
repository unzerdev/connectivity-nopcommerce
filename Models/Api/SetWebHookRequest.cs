using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class SetWebHookRequest : UnzerApiRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
    [JsonPropertyName("event")]
    public string Event { get; set; }
    [JsonPropertyName("eventList")]
    public string[] EventList { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => "v1/webhooks";

    [JsonIgnore]
    public override string Method => HttpMethods.Post;
}
