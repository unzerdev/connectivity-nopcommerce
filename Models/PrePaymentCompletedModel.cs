namespace Unzer.Plugin.Payments.Unzer.Models;
public class PrePaymentCompletedModel
{
    public int OrderId { get; set; }
    public string HowToPay { get; set; }
    public string PaymentReference { get; set; }
    public string Iban { get; set; }
    public string Bic { get; set; }
    public string holder { get; set; }
}
