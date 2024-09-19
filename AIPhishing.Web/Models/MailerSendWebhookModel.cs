using System.Text.Json.Serialization;

namespace AIPhishing.Web.Models;

public class MailerSendWebhookModel
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("data")]
    public MailerSendWebhookDataModel Data { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class MailerSendWebhookDataModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("email")]
    public MailerSendWebhookDataEmailModel Email { get; set; }
}

public class MailerSendWebhookDataEmailModel
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("subject")]
    public string Subject { get; set; }

    [JsonPropertyName("headers")] public string[] Headers { get; set; } = [];

    [JsonPropertyName("tags")] public string[] Tags { get; set; } = [];
}