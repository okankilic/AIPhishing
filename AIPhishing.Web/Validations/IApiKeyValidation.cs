namespace AIPhishing.Web.Validations;

public interface IApiKeyValidation
{
    bool IsValidApiKey(string userApiKey);
}