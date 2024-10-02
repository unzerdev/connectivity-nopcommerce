using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateCustomerRequest : UnzerApiRequest
    {
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
        public override string Path => $"v1/customers";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}

public class Billingaddress
{
    public string name { get; set; }
    public string street { get; set; }
    public string state { get; set; }
    public string zip { get; set; }
    public string city { get; set; }
    public string country { get; set; }
}

public class Shippingaddress
{
    public string name { get; set; }
    public string street { get; set; }
    public string state { get; set; }
    public string zip { get; set; }
    public string city { get; set; }
    public string country { get; set; }
    public string shippingType { get; set; }
}