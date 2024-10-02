namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CapturePayPageResponse : UnzerApiResponse
    {
        public string redirectUrl { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string returnUrl { get; set; }
        public string logoImage { get; set; }
        public string fullPageImage { get; set; }
        public string shopName { get; set; }
        public string shopDescription { get; set; }
        public string tagline { get; set; }
        //public string css { get; set; }
        public string orderId { get; set; }
        public string termsAndConditionUrl { get; set; }
        public string privacyPolicyUrl { get; set; }
        public string paymentReference { get; set; }
        public string impressumUrl { get; set; }
        public string imprintUrl { get; set; }
        public string helpUrl { get; set; }
        public string contactUrl { get; set; }
        public string invoiceId { get; set; }
        public bool card3ds { get; set; }
        public string billingAddressRequired { get; set; }
        public string shippingAddressRequired { get; set; }
        public Additionalattributes additionalAttributes { get; set; }
        public Resources resources { get; set; }
        public string action { get; set; }
    }
}
