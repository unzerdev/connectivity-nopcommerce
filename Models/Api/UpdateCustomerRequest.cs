using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class UpdateCustomerRequest : UnzerApiRequest
{
    public string id { get; set; }
    public string lastname { get; set; }
    public string firstname { get; set; }
    public string salutation { get; set; }
    public string company { get; set; }
    public string customerId { get; set; }
    public string birthDate { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string mobile { get; set; }
    public string language { get; set; }
    public Billingaddress billingAddress { get; set; }
    public Shippingaddress shippingAddress { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => $"v1/customers/{id}";

    [JsonIgnore]
    public override string Method => HttpMethods.Put;
}