using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class CreateV2BasketRequest : UnzerApiRequest
{
    public string id { get; set; }
    public decimal totalValueGross { get; set; }
    public string currencyCode { get; set; }
    public string orderId { get; set; }
    public V2Basketitem[] basketItems { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => $"v2/baskets";

    [JsonIgnore]
    public override string Method => HttpMethods.Post;
}

public class V2Basketitem
{
    public string basketItemReferenceId { get; set; }
    public int quantity { get; set; }
    public decimal vat { get; set; }
    public decimal amountDiscountPerUnitGross { get; set; }
    public decimal amountPerUnitGross { get; set; }
    public string title { get; set; }
    public string type { get; set; }
    public string unit { get; set; }
    public string subTitle { get; set; }
    public string imageUrl { get; set; }
}
