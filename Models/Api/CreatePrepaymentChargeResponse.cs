namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class CreatePrepaymentChargeResponse : UnzerApiResponse
{
    public bool card3ds { get; set; }
    public Message message { get; set; }
    public string amount { get; set; }
    public string currency { get; set; }
    public string returnUrl { get; set; }
    public string date { get; set; }
    public Resources resources { get; set; }
    public string paymentReference { get; set; }
    public Processing processing { get; set; }
}