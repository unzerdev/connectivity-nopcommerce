namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class CreateCustomerResponse : UnzerApiResponse
{
    public string lastname { get; set; }
    public string firstname { get; set; }
    public string salutation { get; set; }
    public string company { get; set; }
    public string customerId { get; set; }
    public string birthDate { get; set; }
    public string email { get; set; }
    public string phone { get; set; }
    public string mobile { get; set; }
    public string language { get; set; }
    public Billingaddress billingAddress { get; set; }
    public Shippingaddress shippingAddress { get; set; }
    public Geolocation geoLocation { get; set; }
}

public class Geolocation
{
    public string clientIp { get; set; }
    public string countryIsoA2 { get; set; }
}


