using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class GetWebHooksRequest : UnzerApiRequest
{
    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => "v1/webhooks";

    [JsonIgnore]
    public override string Method => HttpMethods.Get;
}
