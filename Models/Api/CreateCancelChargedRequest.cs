using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateCancelChargedRequest : UnzerApiRequest
    {
        [JsonIgnore]
        public string paymentId { get; set; }
        [JsonIgnore]
        public string chargeId { get; set; }

        public decimal amount { get; set; }
        public string paymentReference { get; set; }

        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => $"v1/payments/{paymentId}/charges/{chargeId}/cancels";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}