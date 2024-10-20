using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Core.Events;
using Nop.Services.Attributes;
using Nop.Services.Blogs;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.News;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Seo;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Unzer.Plugin.Payments.Unzer.Models;

namespace Unzer.Plugin.Payments.Unzer.Services;
public class UnzerMessageTokenProvider : MessageTokenProvider
{
    protected readonly IGiftCardService _giftCardService;
    protected readonly ILocalizationService _localizationService;
    protected readonly IPriceFormatter _priceFormatter;
    protected readonly IStoreContext _storeContext;
    
    public UnzerMessageTokenProvider(CatalogSettings catalogSettings, CurrencySettings currencySettings, IActionContextAccessor actionContextAccessor, IAddressService addressService, IAttributeFormatter<AddressAttribute, AddressAttributeValue> addressAttributeFormatter, IAttributeFormatter<CustomerAttribute, CustomerAttributeValue> customerAttributeFormatter, IAttributeFormatter<VendorAttribute, VendorAttributeValue> vendorAttributeFormatter, IBlogService blogService, ICountryService countryService, ICurrencyService currencyService, ICustomerService customerService, IDateTimeHelper dateTimeHelper, IEventPublisher eventPublisher, IGenericAttributeService genericAttributeService, IGiftCardService giftCardService, IHtmlFormatter htmlFormatter, ILanguageService languageService, ILocalizationService localizationService, ILogger logger, INewsService newsService, IOrderService orderService, IPaymentPluginManager paymentPluginManager, IPaymentService paymentService, IPriceFormatter priceFormatter, IProductService productService, IRewardPointService rewardPointService, IShipmentService shipmentService, IStateProvinceService stateProvinceService, IStoreContext storeContext, IStoreService storeService, IUrlHelperFactory urlHelperFactory, IUrlRecordService urlRecordService, IWorkContext workContext, MessageTemplatesSettings templatesSettings, PaymentSettings paymentSettings, StoreInformationSettings storeInformationSettings, TaxSettings taxSettings) : base(catalogSettings, currencySettings, actionContextAccessor, addressService, addressAttributeFormatter, customerAttributeFormatter, vendorAttributeFormatter, blogService, countryService, currencyService, customerService, dateTimeHelper, eventPublisher, genericAttributeService, giftCardService, htmlFormatter, languageService, localizationService, logger, newsService, orderService, paymentPluginManager, paymentService, priceFormatter, productService, rewardPointService, shipmentService, stateProvinceService, storeContext, storeService, urlHelperFactory, urlRecordService, workContext, templatesSettings, paymentSettings, storeInformationSettings, taxSettings)
    {
        _priceFormatter = priceFormatter;
        _giftCardService = giftCardService;
        _localizationService = localizationService;
        _storeContext = storeContext;
    }

    protected override async Task WriteTotalsAsync(Order order, Language language, StringBuilder sb)
    {
        if (!order.PaymentMethodSystemName.StartsWith(UnzerPaymentDefaults.SystemName))
        {
            await base.WriteTotalsAsync(order, language, sb);
            return;
        }

        var unzerPaymentType = UnzerPaymentDefaults.ReadUnzerPaymentType(order.PaymentMethodSystemName);
        if (!unzerPaymentType.Prepayment)
        {
            await base.WriteTotalsAsync(order, language, sb);
            return;
        }

        //Unzer prepayment instruction
        var store = await _storeContext.GetCurrentStoreAsync();
        var instructionJson = await _genericAttributeService.GetAttributeAsync<string>(order, UnzerPaymentDefaults.PrePaymentInstructionAttribute, store.Id);
        if (string.IsNullOrEmpty(instructionJson))
        {
            await base.WriteTotalsAsync(order, language, sb);
            return;
        }

        var prePaymentInstModel = JsonSerializer.Deserialize<PrePaymentCompletedModel>(instructionJson);

        //subtotal
        string cusSubTotal;
        var displaySubTotalDiscount = false;
        var cusSubTotalDiscount = string.Empty;
        var languageId = language.Id;
        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
        {
            //including tax

            //subtotal
            var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
            cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            //discount (applied to order subtotal)
            var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
            if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
            {
                cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
                displaySubTotalDiscount = true;
            }
        }
        else
        {
            //excluding tax

            //subtotal
            var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
            cusSubTotal = await _priceFormatter.FormatPriceAsync(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            //discount (applied to order subtotal)
            var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
            if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
            {
                cusSubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
                displaySubTotalDiscount = true;
            }
        }

        //shipping, payment method fee
        string cusShipTotal;
        string cusPaymentMethodAdditionalFee;
        var taxRates = new SortedDictionary<decimal, decimal>();
        var cusTaxTotal = string.Empty;
        var cusDiscount = string.Empty;
        if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            //including tax

            //shipping
            var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
            cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
            //payment method additional fee
            var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
            cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, true);
        }
        else
        {
            //excluding tax

            //shipping
            var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
            cusShipTotal = await _priceFormatter.FormatShippingPriceAsync(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
            //payment method additional fee
            var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
            cusPaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, languageId, false);
        }

        //shipping
        var displayShipping = order.ShippingStatus != ShippingStatus.ShippingNotRequired;

        //payment method fee
        var displayPaymentMethodFee = order.PaymentMethodAdditionalFeeExclTax > decimal.Zero;

        //tax
        bool displayTax;
        bool displayTaxRates;
        if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
        {
            displayTax = false;
            displayTaxRates = false;
        }
        else
        {
            if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                taxRates = new SortedDictionary<decimal, decimal>();
                foreach (var tr in _orderService.ParseTaxRates(order, order.TaxRates))
                    taxRates.Add(tr.Key, _currencyService.ConvertCurrency(tr.Value, order.CurrencyRate));

                displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                displayTax = !displayTaxRates;

                var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                var taxStr = await _priceFormatter.FormatPriceAsync(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode,
                    false, languageId);
                cusTaxTotal = taxStr;
            }
        }

        //discount
        var displayDiscount = false;
        if (order.OrderDiscount > decimal.Zero)
        {
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            cusDiscount = await _priceFormatter.FormatPriceAsync(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);
            displayDiscount = true;
        }

        //total
        var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
        var cusTotal = await _priceFormatter.FormatPriceAsync(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, languageId);

        //Build prepayment instructions
        var prepayInst = BuildPrePaymentInstructions(prePaymentInstModel);

        //subtotal
        sb.AppendLine($"<tr style=\"text-align:left;\">{prepayInst}<td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.SubTotal", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusSubTotal}</strong></td></tr>");

        //discount (applied to order subtotal)
        if (displaySubTotalDiscount)
        {
            sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.SubTotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusSubTotalDiscount}</strong></td></tr>");
        }

        //shipping
        if (displayShipping)
        {
            sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Shipping", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusShipTotal}</strong></td></tr>");
        }

        //payment method fee
        if (displayPaymentMethodFee)
        {
            var paymentMethodFeeTitle = await _localizationService.GetResourceAsync("Messages.Order.PaymentMethodAdditionalFee", languageId);
            sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{paymentMethodFeeTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusPaymentMethodAdditionalFee}</strong></td></tr>");
        }

        //tax
        if (displayTax)
        {
            sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.Tax", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTaxTotal}</strong></td></tr>");
        }

        if (displayTaxRates)
        {
            foreach (var item in taxRates)
            {
                var taxRate = string.Format(await _localizationService.GetResourceAsync("Messages.Order.TaxRateLine"),
                    _priceFormatter.FormatTaxRate(item.Key));
                var taxValue = await _priceFormatter.FormatPriceAsync(item.Value, true, order.CustomerCurrencyCode, false, languageId);
                sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxRate}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{taxValue}</strong></td></tr>");
            }
        }

        //discount
        if (displayDiscount)
        {
            sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.TotalDiscount", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusDiscount}</strong></td></tr>");
        }

        //gift cards
        foreach (var gcuh in await _giftCardService.GetGiftCardUsageHistoryAsync(order))
        {
            var giftCardText = string.Format(await _localizationService.GetResourceAsync("Messages.Order.GiftCardInfo", languageId),
                WebUtility.HtmlEncode((await _giftCardService.GetGiftCardByIdAsync(gcuh.GiftCardId))?.GiftCardCouponCode));
            var giftCardAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate), true, order.CustomerCurrencyCode,
                false, languageId);
            sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardText}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{giftCardAmount}</strong></td></tr>");
        }

        //reward points
        if (order.RedeemedRewardPointsEntryId.HasValue && await _rewardPointService.GetRewardPointsHistoryEntryByIdAsync(order.RedeemedRewardPointsEntryId.Value) is RewardPointsHistory redeemedRewardPointsEntry)
        {
            var rpTitle = string.Format(await _localizationService.GetResourceAsync("Messages.Order.RewardPoints", languageId),
                -redeemedRewardPointsEntry.Points);
            var rpAmount = await _priceFormatter.FormatPriceAsync(-_currencyService.ConvertCurrency(redeemedRewardPointsEntry.UsedAmount, order.CurrencyRate), true,
                order.CustomerCurrencyCode, false, languageId);
            sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpTitle}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{rpAmount}</strong></td></tr>");
        }

        //total
        sb.AppendLine($"<tr style=\"text-align:left;\"><td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{await _localizationService.GetResourceAsync("Messages.Order.OrderTotal", languageId)}</strong></td> <td style=\"background-color: {_templatesSettings.Color3};padding:0.6em 0.4 em;\"><strong>{cusTotal}</strong></td></tr>");
    }

    private string BuildPrePaymentInstructions(PrePaymentCompletedModel instruction)
    {
        var sb = new StringBuilder();

        sb.AppendLine("<td colspan=\"2\" rowspan=\"5\">");
        sb.AppendLine($"{instruction.HowToPay}<br>");
        sb.AppendLine($"{instruction.holder}<br>");
        sb.AppendLine($"{instruction.Iban}<br>");
        sb.AppendLine($"{instruction.Bic}<br>");
        sb.AppendLine($"{instruction.PaymentReference}<br>");
        sb.AppendLine("</td>");

        return sb.ToString();
    }
}
