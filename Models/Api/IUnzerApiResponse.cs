namespace Unzer.Plugin.Payments.Unzer.Models.Api
{
    public interface IUnzerApiResponse
    {
        public string Id { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsPending { get; set; }
        public bool IsError { get; set; }        
    }
}