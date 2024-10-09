using System.Globalization;
using Nop.Core;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Unzer.Plugin.Payments.Unzer.Models.Api;

namespace Unzer.Plugin.Payments.Unzer.Infrastructure
{
    public class UnzerPaymentRequestBuilder
    {
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICurrencyService _currencyService;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly UnzerPaymentSettings _unzerPaymentSettings;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly ICustomerService _customerService;
        private readonly ILanguageService _languageService;

        public UnzerPaymentRequestBuilder(IAddressService addressService, ICountryService countryService, IStateProvinceService stateService, IOrderService orderService, IProductService productService, ICurrencyService currencyService, IStoreService storeService, IStoreContext storeContext, ShoppingCartSettings shoppingCartSettings, UnzerPaymentSettings unzserPaymentSettings, IPaymentPluginManager paymentPluginManager, ICustomerService customerService, ILanguageService languageService)
        {
            _addressService = addressService;
            _countryService = countryService;
            _stateService = stateService;
            _orderService = orderService;
            _productService = productService;
            _currencyService = currencyService;
            _storeService = storeService;
            _storeContext = storeContext;
            _shoppingCartSettings = shoppingCartSettings;
            _unzerPaymentSettings = unzserPaymentSettings;
            _paymentPluginManager = paymentPluginManager;
            _customerService = customerService;
            _languageService = languageService;
        }

        public async Task<CreateAuthorizePayPageRequest> BuildAuthorizePayPageRequestAsync(Order order, bool isRecurring, string unzerCustomerId, string basketId)
        {
            var shopUrl = await GetShopUrlAsync();
            var returnUrl = $"{shopUrl}/unzerpayment/unzerpaymentstatus/{order.Id}";
            //var returnUrl = $"{shopUrl}/checkout/completed/{order.Id}";
            var currentStore = _storeContext.GetCurrentStore();

            var currencyCode = _unzerPaymentSettings.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
            {
                currencyCode = order.CustomerCurrencyCode;
            }

            var orderTotal = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                orderTotal = Math.Round(orderTotal, 2);
            }

            var unzerPaymentType = UnzerPaymentDefaults.ReadUnzerPaymentType(order.PaymentMethodSystemName);
            var selectedPaymentMethod = unzerPaymentType != null ? unzerPaymentType.UnzerName : order.PaymentMethodSystemName;

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var excludeTypes = _unzerPaymentSettings.SelectedPaymentTypes.Count > 1 ? _unzerPaymentSettings.AvailablePaymentTypes.Where(t => t != selectedPaymentMethod).ToArray() : new string[0];

            var authReq = new CreateAuthorizePayPageRequest
            {
                currency = currencyCode,
                amount = orderTotal.ToString("0.00", CultureInfo.InvariantCulture),
                orderId = order.Id.ToString("D6"),
                returnUrl = returnUrl,
                card3ds = true,
                logoImage = _unzerPaymentSettings.LogoImage,
                shopName = currentStore.Name,
                shopDescription = _unzerPaymentSettings.ShopDescription,
                tagLine = _unzerPaymentSettings.TagLine,
                additionalAttributes = isRecurring ? new AdditionalAttributes { recurrenceType = "scheduled" } : null,
                excludeTypes = excludeTypes,
                resources = new Resources
                {
                    customerId = !string.IsNullOrEmpty(unzerCustomerId) ? customer.CustomerGuid.ToString() : null,
                    metadataId = _unzerPaymentSettings.UnzerMetadataId,                    
                    basketId = !string.IsNullOrEmpty(basketId) ? basketId : null
                }                
            };       

            return authReq;
        }

        public async Task<CreateCapturePayPageRequest> BuildCapturePayPageRequestAsync(Order order, bool isRecurring, string unzerCustomerId, string basketId)
        {
            var shopUrl = await GetShopUrlAsync();
            var returnUrl = $"{shopUrl}/unzerpayment/unzerpaymentstatus/{order.Id}";
            //var returnUrl = $"{shopUrl}/checkout/completed/{order.Id}";
            var currentStore = _storeContext.GetCurrentStore();

            var currencyCode = _unzerPaymentSettings.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
            {
                currencyCode = order.CustomerCurrencyCode;
            }

            var orderTotal = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
            {             
                orderTotal = Math.Round(orderTotal, 2);
            }

            var unzerPaymentType = UnzerPaymentDefaults.ReadUnzerPaymentType(order.PaymentMethodSystemName);
            var selectedPaymentMethod = unzerPaymentType != null ? unzerPaymentType.UnzerName : order.PaymentMethodSystemName;

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);
            var excludeTypes = _unzerPaymentSettings.SelectedPaymentTypes.Count > 1 ? _unzerPaymentSettings.AvailablePaymentTypes.Where(t => t != selectedPaymentMethod).ToArray() : new string[0];

            var authReq = new CreateCapturePayPageRequest
            {
                currency = currencyCode,
                amount = orderTotal.ToString("0.00", CultureInfo.InvariantCulture),
                orderId = order.Id.ToString("D6"),
                returnUrl = returnUrl,
                card3ds = true,
                logoImage = _unzerPaymentSettings.LogoImage,
                shopName = currentStore.Name,
                shopDescription = _unzerPaymentSettings.ShopDescription,
                tagLine = _unzerPaymentSettings.TagLine,
                additionalAttributes = isRecurring ? new AdditionalAttributes { recurrenceType = "scheduled" } : null,
                excludeTypes = excludeTypes,
                resources = new Resources
                {
                    customerId = !string.IsNullOrEmpty(unzerCustomerId) ? customer.CustomerGuid.ToString() : null,
                    metadataId = _unzerPaymentSettings.UnzerMetadataId,
                    basketId = !string.IsNullOrEmpty(basketId) ? basketId : null
                }
            };

            return authReq;
        }

        public async Task<CreateCaptureRequest> BuildCaptureRequestAsync(Order order, decimal amount)
        {
            var paymentInfo = order.AuthorizationTransactionId.Split('/');
            var paymentId = paymentInfo.Any() ? paymentInfo.First() : order.Id.ToString("D6");
            var capReq = new CreateCaptureRequest
            {
                paymentId = paymentId,
                amount = amount.ToString("0.00", CultureInfo.InvariantCulture),
                orderId = order.Id.ToString("D6"),
            };            

            return capReq;
        }

        public async Task<CreateSubCaptureRequest> BuildSubCaptureRequestAsync(Order order, decimal amount)
        {
            var capReq = new CreateSubCaptureRequest
            {
                ChargeId = order.SubscriptionTransactionId,
                orderId = order.Id.ToString("D6"),
                amount = amount.ToString("0.00", CultureInfo.InvariantCulture),
            };

            return capReq;
        }

        public async Task<CreateRefundRequest> BuildRefundRequestAsync(Order order, decimal amount)
        {
            var paymentInfo = order.CaptureTransactionId.Split('/');
            var paymentId = paymentInfo.Any() ? paymentInfo.First() : order.Id.ToString("D6");
            var captureId = paymentInfo.Any() ? paymentInfo.Last() : order.CaptureTransactionId;

            var refReq = new CreateRefundRequest
            {
                chargeId = captureId,
                paymentId = paymentId,
                amount = amount.ToString("0.00", CultureInfo.InvariantCulture),
            };

            return refReq;
        }

        public async Task<CreateCancelAuthorizedRequest> BuildCancelRequestAsync(Order order, decimal amount)
        {
            var paymentInfo = order.AuthorizationTransactionId.Split('/');
            var paymentId = paymentInfo.Any() ? paymentInfo.First() : order.Id.ToString("D6");
            var authId = paymentInfo.Any() ? paymentInfo.Last() : order.AuthorizationTransactionId;

            var canReq = new CreateCancelAuthorizedRequest
            {
                authorizeId = authId,
                paymentId = paymentId,
                amount = amount.ToString("0.00", CultureInfo.InvariantCulture),
            };

            return canReq;
        }

        public async Task<CreateMetadataRequest> BuildCreateMetadataRequestAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var unzerPlugin = await _paymentPluginManager.LoadPluginBySystemNameAsync(UnzerPaymentDefaults.SystemName);            

            var metaReq = new CreateMetadataRequest
            {
                pluginType = UnzerPaymentDefaults.MetadataPluginType,
                shopType = UnzerPaymentDefaults.MetadataShopType,
                pluginVersion = unzerPlugin.PluginDescriptor.Version,
                shopVersion = unzerPlugin.PluginDescriptor.SupportedVersions.FirstOrDefault()
            };

            return metaReq;
        }

        public async Task<UpdateMetadataRequest> BuildUpdateMetadataRequestAsync(string metadatId)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var unzerPlugin = await _paymentPluginManager.LoadPluginBySystemNameAsync(UnzerPaymentDefaults.SystemName);

            var metaReq = new UpdateMetadataRequest
            {
                metadataId = metadatId,
                pluginType = UnzerPaymentDefaults.MetadataPluginType,
                shopType = UnzerPaymentDefaults.MetadataShopType,
                pluginVersion = unzerPlugin.PluginDescriptor.Version,
                shopVersion = unzerPlugin.PluginDescriptor.SupportedVersions.FirstOrDefault()
            };

            return metaReq;
        }

        public async Task<CreateCustomerRequest> BuildCreateCustomerRequestAsync(Customer customer, Address billingAddress, Address shippingAddress)
        {
            var billingCountry = await _countryService.GetCountryByIdAsync(billingAddress.CountryId.Value);
            var billingState = await _stateService.GetStateProvinceByIdAsync(billingAddress.StateProvinceId.Value);
            var shippingCountry = await _countryService.GetCountryByIdAsync(shippingAddress.CountryId.Value);
            var shippingState = await _stateService.GetStateProvinceByIdAsync(shippingAddress.StateProvinceId.Value);
            var customerLanguage = customer.LanguageId.HasValue ? (await _languageService.GetLanguageByIdAsync(customer.LanguageId.Value))?.UniqueSeoCode : string.Empty;

            var custIsGuset = await _customerService.IsGuestAsync(customer);

            var custReq = new CreateCustomerRequest
            {
                customerId = customer.CustomerGuid.ToString(),
                firstname = !custIsGuset ? customer.FirstName : billingAddress.FirstName,
                lastname = !custIsGuset ? customer.LastName : billingAddress.LastName,
                company = !custIsGuset ? customer.Company : billingAddress.Company,
                email = !custIsGuset ? customer.Email : billingAddress.Email,
                phone = !custIsGuset ? customer.Phone : billingAddress.PhoneNumber,
                mobile = !custIsGuset ? customer.Phone : billingAddress.PhoneNumber,
                language = customerLanguage,
                billingAddress = new Billingaddress
                {
                    name = $"{billingAddress.FirstName} {billingAddress.LastName}",
                    street = billingAddress.Address1,
                    city = billingAddress.City,
                    zip = billingAddress.ZipPostalCode,
                    country = billingCountry?.TwoLetterIsoCode,
                    state = billingState?.Name,
                },
                shippingAddress = new Shippingaddress
                {
                    name = $"{shippingAddress.FirstName} {shippingAddress.LastName}",
                    street = shippingAddress.Address1,
                    city = shippingAddress.City,
                    zip = shippingAddress.ZipPostalCode,
                    country = shippingCountry?.TwoLetterIsoCode,
                    state = shippingState?.Name
                }
            };

            return custReq;
        }

        public async Task<UpdateCustomerRequest> BuildUpdateCustomerRequestAsync(string unzerCustomerId, Customer customer, Address billingAddress, Address shippingAddress)
        {
            var billingCountry = await _countryService.GetCountryByIdAsync(billingAddress.CountryId.Value);
            var billingState = await _stateService.GetStateProvinceByIdAsync(billingAddress.StateProvinceId.Value);
            var shippingCountry = await _countryService.GetCountryByIdAsync(shippingAddress.CountryId.Value);
            var shippingState = await _stateService.GetStateProvinceByIdAsync(shippingAddress.StateProvinceId.Value);
            var customerLanguage = customer.LanguageId.HasValue ? (await _languageService.GetLanguageByIdAsync(customer.LanguageId.Value))?.UniqueSeoCode : string.Empty;

            var custIsGuset = await _customerService.IsGuestAsync(customer);

            var custReq = new UpdateCustomerRequest
            {
                id = unzerCustomerId,
                customerId = customer.CustomerGuid.ToString(),
                firstname = !custIsGuset ? customer.FirstName : billingAddress.FirstName,
                lastname = !custIsGuset ? customer.LastName : billingAddress.LastName,
                company = !custIsGuset ? customer.Company : billingAddress.Company,
                email = !custIsGuset ? customer.Email : billingAddress.Email,
                phone = !custIsGuset ? customer.Phone : billingAddress.PhoneNumber,
                mobile = !custIsGuset ? customer.Phone : billingAddress.PhoneNumber,
                language = customerLanguage,
                billingAddress = new Billingaddress
                {
                    name = $"{billingAddress.FirstName} {billingAddress.LastName}",
                    street = billingAddress.Address1,
                    city = billingAddress.City,
                    zip = billingAddress.ZipPostalCode,
                    country = billingCountry?.TwoLetterIsoCode,
                    state = billingState?.Name,
                },
                shippingAddress = new Shippingaddress
                {
                    name = $"{shippingAddress.FirstName} {shippingAddress.LastName}",
                    street = shippingAddress.Address1,
                    city = shippingAddress.City,
                    zip = shippingAddress.ZipPostalCode,
                    country = shippingCountry?.TwoLetterIsoCode,
                    state = shippingState?.Name
                }
            };

            return custReq;
        }

        public async Task<CreateBasketRequest> BuildCreateBasketRequestAsync(Order order)
        {
            var currencyCode = _unzerPaymentSettings.CurrencyCode;
            if (string.IsNullOrEmpty(currencyCode))
            {
                currencyCode = order.CustomerCurrencyCode;
            }

            var orderItems = await _orderService.GetOrderItemsAsync(order.Id);

            var basketReq = new CreateBasketRequest
            {
                amountTotalGross = order.OrderSubtotalExclTax,
                amountTotalDiscount = order.OrderSubTotalDiscountExclTax,
                amountTotalVat = order.OrderTax,
                currencyCode = currencyCode,
                orderId = order.Id.ToString("D6"),
                basketItems = await orderItems.SelectAwait( async i => new Basketitem
                {
                    basketItemReferenceId = i.Id.ToString(),
                    quantity = i.Quantity,
                    amountGross = i.PriceInclTax,
                    amountPerUnit = i.UnitPriceExclTax,
                    amountDiscount = i.DiscountAmountInclTax,
                    amountNet = i.PriceExclTax,
                    amountVat = i.PriceInclTax - i.PriceExclTax,
                    title = (await _productService.GetProductByIdAsync(i.ProductId)).Name

                }).ToArrayAsync()
            };

            return basketReq;
        }

        public async Task<GetPaymentAutorizeRequest> BuildPaymentAuthorizeRequestAsync(string paymentId)
        {
            var payAuthReq = new GetPaymentAutorizeRequest
            {
                paymentId = paymentId
            };

            return payAuthReq;
        }

        public async Task<GetPaymentCaptureRequest> BuildPaymentCaptureRequestAsync(string paymentId)
        {
            var payCaptReq = new GetPaymentCaptureRequest
            {
                paymentId = paymentId
            };

            return payCaptReq;
        }

        public async Task<SetWebHookRequest> BuildWebHookRequestAsync(string url, WebHookEventType eventType)
        {
            var setWebHook = new SetWebHookRequest
            {
                Url = url,
                Event = eventType.ToString(),
            };

            return setWebHook;
        }

        public async Task<SetWebHookRequest> BuildWebHookRequestAsync(string url, List<string> eventTypes)
        {
            var setWebHook = new SetWebHookRequest
            {
                Url = url,
                EventList = eventTypes.ToArray()
            };

            return setWebHook;
        }


        private async Task<string> GetShopUrlAsync()
        {
            string shopUrl = _unzerPaymentSettings.ShopUrl;
            var curStore = await _storeContext.GetCurrentStoreAsync();

            if ((await _storeService.GetAllStoresAsync()).Count > 1 || string.IsNullOrEmpty(shopUrl))
            {
                shopUrl = curStore.Url;
            }

            if (!curStore.SslEnabled && !shopUrl.StartsWith("http://"))
            {
                shopUrl = string.Format("http://{0}", shopUrl);
            }
            else if (curStore.SslEnabled && !shopUrl.StartsWith("https://"))
            {
                if (shopUrl.StartsWith("http://"))
                {
                    shopUrl = shopUrl.Replace("http://", "https://");
                }
                else
                {
                    shopUrl = string.Format("https://{0}", shopUrl);
                }
            }

            return shopUrl.TrimEnd('/');
        }

    }
}
