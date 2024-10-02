using System.Text.Json.Serialization;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class UnzerCallbackPayload
{
    [JsonPropertyName("event")]
    public string Event { get; set; }
    public string publicKey { get; set; }
    public string retrieveUrl { get; set; }
    public string paymentId { get; set; }
}