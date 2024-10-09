using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Unzer.Plugin.Payments.Unzer.Infrastructure;
using Unzer.Plugin.Payments.Unzer.Models.Api;

namespace Unzer.Plugin.Payments.Unzer.Services;
public class CaptureEventHandler : ICallEventHandler<CaptureEventHandler>
{
    private readonly IUnzerApiService _unzerApiService;
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly UnzerPaymentSettings _unzerPaymentSettings;
    private readonly ILogger _logger;

    public CaptureEventHandler(
        IUnzerApiService unzerApiService,
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        UnzerPaymentSettings unzerPaymentSettings,
        ILogger logger)
    {
        _unzerApiService = unzerApiService;
        _orderService = orderService;
        _orderProcessingService = orderProcessingService;
        _unzerPaymentSettings = unzerPaymentSettings;
        _logger = logger;
    }

    public async Task HandleEvent(UnzerCallbackPayload eventPayload)
    {
        if (UnzerPaymentDefaults.IgnoreCallbackEvents.Contains(eventPayload.Event))
            return;

        var paymentCapt = await _unzerApiService.PaymentCaptureResponse(eventPayload.paymentId);
        if (paymentCapt == null || paymentCapt.IsError)
            throw new NopException(paymentCapt.ErrorResponse.Errors.First().merchantMessage);

        var nopOrder = await _orderService.GetOrderByIdAsync(Convert.ToInt32(paymentCapt.orderId));
        if (nopOrder == null)
            throw new NopException($"Order {paymentCapt.orderId} for payment {eventPayload.paymentId} could not be found");

        if (eventPayload.Event == "charge.succeeded" && nopOrder.PaymentStatus == PaymentStatus.Paid)
            return;

        if (paymentCapt.IsError)
        {
            await _logger.WarningAsync($"CaptureEventHandler.UpdatePaymentAsync: Capture for order {nopOrder.Id} failed at provider");
            await AddOrderNote(nopOrder, "Order cannot be captured, failed at provider");
            return;
        }

        await UpdatePaymentAsync(nopOrder, paymentCapt);
    }

    private async Task UpdatePaymentAsync(Order order, PaymentCaptureResponse paymentCapt)
    {
        var isRecurrence = paymentCapt.additionalTransactionData?.card?.recurrenceType == "scheduled";
        var isAutoCapture = _unzerPaymentSettings.AutoCapture == AutoCapture.AutoCapture ||
                            _unzerPaymentSettings.AutoCapture == AutoCapture.OnAuthForNoneDeliverProduct ||
                            _unzerPaymentSettings.AutoCapture == AutoCapture.OnAuthForDownloadableProduct;

        if ((isAutoCapture || isRecurrence) && order.PaymentStatus == PaymentStatus.Pending)
        {
            if (!paymentCapt.IsPending && this._orderProcessingService.CanMarkOrderAsAuthorized(order))
            {
                await _orderProcessingService.MarkAsAuthorizedAsync(order);
            }
        }

        if (!await _orderProcessingService.CanCaptureAsync(order))
        {
            await _logger.WarningAsync($"CaptureEventHandler.UpdatePaymentAsync: Capture for order {order.Id} cannot be handled, order must be authorized");
            await AddOrderNote(order, "Order cannot be captured, must be authorized");
        }
        else
        {
            var captAndPayID = $"{paymentCapt.resources.paymentId}/{paymentCapt.Id}";
            order.CaptureTransactionId = captAndPayID;
            order.CaptureTransactionResult = paymentCapt.message.merchant;
            order.SubscriptionTransactionId = isRecurrence ? paymentCapt.resources?.typeId : null;
            await _orderService.UpdateOrderAsync(order);

            if (_orderProcessingService.CanMarkOrderAsPaid(order))
            {
                await _orderProcessingService.MarkOrderAsPaidAsync(order);
            }
            else
            {
                await _logger.WarningAsync($"CaptureEventHandler.UpdatePaymentAsync: Captured order {order.Id} cannot be marked as paid");
                await AddOrderNote(order, "Order cannot be marked as paid");
            }
        }
    }

    private async Task AddOrderNote(Order order, string note)
    {
        await _orderService.InsertOrderNoteAsync(new OrderNote
        {
            OrderId = order.Id,
            Note = note,
            DisplayToCustomer = false,
            CreatedOnUtc = DateTime.UtcNow
        });
    }
}
