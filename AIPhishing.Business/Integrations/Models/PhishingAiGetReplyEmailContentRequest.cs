using System.Text.Json.Serialization;

namespace AIPhishing.Business.Integrations.Models;

public record PhishingAiGetReplyEmailContentRequest(
    [property: JsonPropertyName("targetFullName")] string TargetFullName, 
    [property: JsonPropertyName("targetEmail")] string TargetEmail, 
    [property: JsonPropertyName("emailId")] Guid EmailId,
    [property: JsonPropertyName("linkUrl")] string LinkUrl,
    [property: JsonPropertyName("subject")] string Subject, 
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("conversationId")] Guid ConversationId);