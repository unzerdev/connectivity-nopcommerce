using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class CreatePrepaymentChargeRequest : UnzerApiRequest
{
    public decimal amount { get; set; }
    public string currency { get; set; }
    public string paymentReference { get; set; }
    public string orderId { get; set; }
    public string invoiceId { get; set; }
    public Resources resources { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => "v1/payments/charges";

    [JsonIgnore]
    public override string Method => HttpMethods.Post;
}
