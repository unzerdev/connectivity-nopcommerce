namespace Unzer.Plugin.Payments.Unzer.Infrastructure
{
    public enum AutoCapture
    {
        None,
        OnOrderShipped,
        OnOrderDelivered,
        AutoCapture,
        OnAuthForDownloadableProduct,
        OnAuthForNoneDeliverProduct
    }

    public enum WebHookEventType
    {
        authorize,
        charge,
        payment,
        chargeback,
        payout
    }
}
