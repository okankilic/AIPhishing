using AIPhishing.Business.Integrations.Models;

namespace AIPhishing.Business.Integrations;

public interface IPhishingAiApiClient
{
    Task<PhishingAiGetEmailContentResponse> CreateEmailContentAsync(string language, PhishingAiGetEmailContentRequest request);
    Task<string> GetReplyEmailContentAsync(string language, PhishingAiGetReplyEmailContentRequest request);
}