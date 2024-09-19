using System.Text.Json.Serialization;

namespace AIPhishing.Business.Integrations.Models;

public record PhishingAiGetEmailContentResponse(
    [property: JsonPropertyName("sender")] string Sender, 
    [property: JsonPropertyName("subject")] string Subject, 
    [property: JsonPropertyName("content")] string Content);