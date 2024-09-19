using Microsoft.VisualBasic;

namespace AIPhishing.Web.Validations;

public class ApiKeyValidation(IConfiguration configuration)
    : IApiKeyValidation
{
    public const string ApiKeyHeaderName = "X-API-Key";
    public const string ApiKeyName = "ApiKey";

    public bool IsValidApiKey(string userApiKey)
    {
        if (string.IsNullOrWhiteSpace(userApiKey))
            return false;
        
        var apiKey = configuration.GetValue<string>(ApiKeyName);
        
        return apiKey != null && apiKey == userApiKey;
    }
}