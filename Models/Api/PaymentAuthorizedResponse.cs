namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class PaymentAuthorizedResponse : UnzerApiResponse
{
    public bool card3ds { get; set; }
    public string redirectUrl { get; set; }
    public Message message { get; set; }
    public string amount { get; set; }
    public string currency { get; set; }
    public string returnUrl { get; set; }
    public string date { get; set; }
    public Resources resources { get; set; }
    public string orderId { get; set; }
    public string invoiceId { get; set; }
    public Processing processing { get; set; }
}

public class Message
{
    public string code { get; set; }
    public string merchant { get; set; }
    public string customer { get; set; }
}

public class Processing
{
    public string uniqueId { get; set; }
    public string shortId { get; set; }
    public string _3dsEci { get; set; }
    public string traceId { get; set; }
    public string iban { get; set; }
    public string bic { get; set; }
    public string descriptor { get; set; }
    public string holder { get; set; }
}
