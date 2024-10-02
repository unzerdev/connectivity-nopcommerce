using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreatePaymentLinkRequest : UnzerApiRequest
    {
        public string alias { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string returnUrl { get; set; }
        public string logoImage { get; set; }
        public string fullPageImage { get; set; }
        public string shopName { get; set; }
        public string shopDescription { get; set; }
        public string tagline { get; set; }
        public Css css { get; set; }
        public string termsAndConditionUrl { get; set; }
        public string privacyPolicyUrl { get; set; }
        public string imprintUrl { get; set; }
        public string helpUrl { get; set; }
        public string contactUrl { get; set; }
        public string card3ds { get; set; }
        public string billingAddressRequired { get; set; }
        public string shippingAddressRequired { get; set; }
        public string orderId { get; set; }
        public string invoiceId { get; set; }
        public string expires { get; set; }
        public string intention { get; set; }
        public string paymentReference { get; set; }
        public Additionalattributes additionalAttributes { get; set; }
        public Resources resources { get; set; }
        public string orderIdRequired { get; set; }
        public string invoiceIdRequired { get; set; }
        public string oneTimeUse { get; set; }
        public string successfullyProcessed { get; set; }
        public string[] excludeTypes { get; set; }
        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => "v1/linkpay/authorize";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }

    public class Css
    {
        public string shopDescription { get; set; }
        public string tagline { get; set; }
        public string stepline { get; set; }
        public string header { get; set; }
        public string shopName { get; set; }
        public string helpUrl { get; set; }
        public string contactUrl { get; set; }
        public string invoiceId { get; set; }
        public string orderId { get; set; }
        public string backToMerchantLink { get; set; }
    }

    public class Additionalattributes
    {
        public string property1 { get; set; }
        public string property2 { get; set; }
    }
}
