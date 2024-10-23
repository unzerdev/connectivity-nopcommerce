using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class GetPaymentCaptureRequest : UnzerApiRequest
{
    [JsonIgnore]
    public string paymentId { get; set; }
    [JsonIgnore]
    public string chargeId { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => $"v1/payments/{paymentId}/charges/{chargeId}";

    [JsonIgnore]
    public override string Method => HttpMethods.Get;
}