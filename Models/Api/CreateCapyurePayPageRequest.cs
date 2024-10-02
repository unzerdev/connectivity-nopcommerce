﻿using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public class CreateCapturePayPageRequest : UnzerApiRequest
    {
        public string amount { get; set; }
        public string currency { get; set; }
        public string returnUrl { get; set; }
        public string orderId { get; set; }
        public bool card3ds { get; set; }
        public string invoiceId { get; set; }
        public string logoImage { get; set; }
        public string shopName { get; set; }
        public string shopDescription { get; set; }
        public string tagLine { get; set; }
        public AdditionalAttributes additionalAttributes { get; set; }
        public Resources resources { get; set; }
        public string[] excludeTypes { get; set; }

        public string paymentReference { get; set; }

        [JsonIgnore]
        public override string BaseUrl => UnzerPaymentDefaults.UnzerApiUrl;

        [JsonIgnore]
        public override string Path => "v1/paypage/charge";

        [JsonIgnore]
        public override string Method => HttpMethods.Post;
    }
}
