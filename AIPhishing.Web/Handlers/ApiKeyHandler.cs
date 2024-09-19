using AIPhishing.Web.Requirements;
using AIPhishing.Web.Validations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;

namespace AIPhishing.Web.Handlers;

public class ApiKeyHandler(IHttpContextAccessor httpContextAccessor, IApiKeyValidation apiKeyValidation)
    : AuthorizationHandler<ApiKeyRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, ApiKeyRequirement requirement)
    {
        var apiKey = httpContextAccessor?.HttpContext?.Request.Headers[ApiKeyValidation.ApiKeyHeaderName].ToString();
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        if (!apiKeyValidation.IsValidApiKey(apiKey))
        {
            context.Fail();
            return Task.CompletedTask;
        }
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}