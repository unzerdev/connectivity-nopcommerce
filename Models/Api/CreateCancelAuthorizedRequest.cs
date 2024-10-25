using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateCancelAuthorizedRequest : UnzerApiRequest
    {
        [JsonIgnore]
        public string paymentId { get; set; }
        [JsonIgnore]
        public string authorizeId { get; set; }

        public decimal amount { get; set; }
        public string paymentReference { get; set; }

        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => $"v1/payments/{paymentId}/authorize/{authorizeId}/cancels";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}