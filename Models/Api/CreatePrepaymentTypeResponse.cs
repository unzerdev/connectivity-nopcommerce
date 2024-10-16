namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class CreatePrepaymentTypeResponse : UnzerApiResponse
{
    public string method { get; set; }
    public bool recurring { get; set; }
    public Geolocation geoLocation { get; set; }
}