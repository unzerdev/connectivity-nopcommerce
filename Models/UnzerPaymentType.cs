namespace Unzer.Plugin.Payments.Unzer.Models;
public class UnzerPaymentType
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string SystemName { get; set; }
    public bool SupportAuthurize { get; set; }
    public bool SupportCharge { get; set; }
    public bool Deprecated { get; set; }
}
