using System.Text.Json.Serialization;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public abstract class UnzerApiRequest : IUnzerApiRequest
    {
        [JsonIgnore]
        public abstract string BaseUrl { get; }   

        [JsonIgnore]
        public abstract string Path { get; }

        [JsonIgnore]
        public abstract string Method { get; }
    }
}
