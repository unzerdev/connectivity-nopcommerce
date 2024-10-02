using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateAuthorizeRequest : UnzerApiRequest
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string returnUrl { get; set; }
        public bool card3ds { get; set; }
        public string paymentReference { get; set; }
        public string orderId { get; set; }
        public string invoiceId { get; set; }
        public string effectiveInterestRate { get; set; }
        public Resources resources { get; set; }
        public string linkpayId { get; set; }
        public Additionaltransactiondata additionalTransactionData { get; set; }

        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => "v1/payments/authorize";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}
