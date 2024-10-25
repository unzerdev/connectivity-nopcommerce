using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateSubCaptureRequest : UnzerApiRequest
    {
        [JsonIgnore]
        public string ChargeId { get; set; }

        public decimal amount { get; set; }
        public string currency { get; set; }
        public string returnUrl { get; set; }
        public string orderId { get; set; }
        public Resources resources => new Resources { typeId = ChargeId };
        public Additionaltransactiondata additionalTransactionData => new Additionaltransactiondata
        {
            card = new Card
            {
                recurrenceType = "schedule"
            }
        };

        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => $"v1/payments/charges";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}
