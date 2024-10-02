using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class GetCustomerRequest : UnzerApiRequest
{
    public string customerId { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => $"v1/customers/{customerId}";

    [JsonIgnore]
    public override string Method => HttpMethods.Get;
}
