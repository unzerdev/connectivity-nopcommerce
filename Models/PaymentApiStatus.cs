namespace Unzer.Plugin.Payments.Unzer.Models
{
    public class PaymentApiStatus
    {
		public bool Success { get; set; }
        public string StatusMessage { get; set; }
        public string ResponseId { get; set; }
        public string RawContent { get; set; }
    }
}