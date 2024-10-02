using System.Text.Json.Serialization;

namespace Unzer.Plugin.Payments.Unzer.Models.Api;
public class SetWebHookResponse : WebHookEvent
{
}

public class SetWebHooksResponse : UnzerApiResponse
{
    public SetWebHooksResponse()
    {        
        Events = new List<WebHookEvent>();
    }

    [JsonPropertyName("events")]
    public List<WebHookEvent> Events { get; set; }
}

public class WebHookEvent : UnzerApiResponse
{
    [JsonPropertyName("event")]
    public string Event { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; }
}

