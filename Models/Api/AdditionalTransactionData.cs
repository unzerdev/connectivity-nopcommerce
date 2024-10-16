namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class Resources
    {
        public string customerId { get; set; }
        public string typeId { get; set; }
        public string metadataId { get; set; }
        public string basketId { get; set; }
        public string paymentId { get; set; }
        public string traceId { get; set; }
    }

    public class Additionaltransactiondata
    {
        public Card card { get; set; }
        public Riskdata riskData { get; set; }
        public Shipping shipping { get; set; }
        public Paypal paypal { get; set; }

        public string termsAndConditionUrl { get; set; }
        public string privacyPolicyUrl { get; set; }
    }
    public class AdditionalAttributes
    {
        public decimal? effectiveInterestRate { get; set; }
        public string exemptionType { get; set; }
        public string recurrenceType { get; set; }
    }

    public class Card
    {
        public string recurrenceType { get; set; }
        public string brandTransactionId { get; set; }
        public string settlementDay { get; set; }
        public string exemptionType { get; set; }
        public string liability { get; set; }
        public Authentication authentication { get; set; }
    }

    public class Authentication
    {
        public string verificationId { get; set; }
        public string resultIndicator { get; set; }
        public string dsTransactionId { get; set; }
        public string protocolVersion { get; set; }
        public string authenticationStatus { get; set; }
        public string xId { get; set; }
    }

    public class Riskdata
    {
        public string threatMetrixId { get; set; }
        public string customerGroup { get; set; }
        public string customerId { get; set; }
        public string confirmedAmount { get; set; }
        public string confirmedOrders { get; set; }
        public string internalScore { get; set; }
        public string registrationLevel { get; set; }
        public string registrationDate { get; set; }
    }

    public class Shipping
    {
        public string deliveryTrackingId { get; set; }
        public string deliveryService { get; set; }
        public string returnTrackingId { get; set; }
    }

    public class Paypal
    {
        public string checkoutType { get; set; }
    }

    public class Paylater
    {
        public string targetDueDate { get; set; }
        public string merchantComment { get; set; }
        public string merchantOrderId { get; set; }
    }

}
