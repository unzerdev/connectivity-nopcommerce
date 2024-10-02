using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class CreateBasketRequest : UnzerApiRequest
{
    public decimal amountTotalGross { get; set; }
    public decimal amountTotalDiscount { get; set; }
    public decimal amountTotalVat { get; set; }
    public string currencyCode { get; set; }
    public string orderId { get; set; }
    public string note { get; set; }
    public Basketitem[] basketItems { get; set; }

    [JsonIgnore]
    public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

    [JsonIgnore]
    public override string Path => $"v1/baskets";

    [JsonIgnore]
    public override string Method => HttpMethods.Post;
}

public class Basketitem
{
    public string basketItemReferenceId { get; set; }
    public string unit { get; set; }
    public int quantity { get; set; }
    public decimal amountDiscount { get; set; }
    public int vat { get; set; }
    public decimal amountGross { get; set; }
    public decimal amountVat { get; set; }
    public decimal amountPerUnit { get; set; }
    public decimal amountNet { get; set; }
    public string title { get; set; }
    public string subTitle { get; set; }
    public string imageUrl { get; set; }
    public string type { get; set; }
}
