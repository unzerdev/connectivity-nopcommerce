using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class GetKeyPairResponse : UnzerApiResponse
{
    public string publicKey { get; set; }
    public string secureLevel { get; set; }
    public string merchantName { get; set; }
    public string merchantAddress { get; set; }
    public string[] availablePaymentTypes { get; set; }
    public string validateBasket { get; set; }
    public string captchaFeatureEnable { get; set; }
    public string coreId { get; set; }
}