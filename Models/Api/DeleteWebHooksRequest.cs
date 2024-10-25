using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class DeleteWebHooksRequest : UnzerApiRequest
{
    public string webHookId { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => $"v1/webhooks/{webHookId}";

    [JsonIgnore]
    public override string Method => HttpMethods.Get;
}
