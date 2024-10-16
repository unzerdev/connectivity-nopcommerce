using Unzer.Plugin.Payments.Unzer.Models.Api;

namespace Unzer.Plugin.Payments.Unzer.Services;
public interface ICallEventHandler<T> where T : class, ICallEventHandler<T>
{
    Task HandleEvent(UnzerCallbackPayload eventPayload);
    Task HandleEvent(PaymentCaptureResponse paymentResponse);
}
