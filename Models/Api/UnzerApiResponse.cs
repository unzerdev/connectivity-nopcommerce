using System.Net;
using System.Text.Json.Serialization;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class UnzerApiResponse : IUnzerApiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsPending { get; set; }
        public bool IsError { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        public UnzerApiErrorResponse ErrorResponse { get; set; }
    }
}
