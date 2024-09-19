using System.Text.Json.Serialization;

namespace AIPhishing.Business.Integrations.Models;

public record PhishingAiGetEmailContentRequest(
    [property: JsonPropertyName("attackType")] string AttackType, 
    [property: JsonPropertyName("targetFullName")] string TargetFullName, 
    [property: JsonPropertyName("targetEmail")] string TargetEmail, 
    [property: JsonPropertyName("emailId")] Guid EmailId, 
    [property: JsonPropertyName("linkUrl")] string LinkUrl);