using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreatePaymentRequest : UnzerApiRequest
    {
        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => "";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;        
    }
}
