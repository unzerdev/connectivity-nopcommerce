using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateCaptureRequest : UnzerApiRequest
    {
        [JsonIgnore]
        public string paymentId { get; set; }

        public decimal amount { get; set; }
        public string orderId { get; set; }
        public string invoiceId { get; set; }
        public string paymentReference { get; set; }

        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => $"v1/payments/{paymentId}/charges";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}
