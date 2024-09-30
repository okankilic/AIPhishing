using AIPhishing.Business.Contexts;
using AIPhishing.Common.Constants;
using AIPhishing.WebAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

[Authorize(Policy = "ApiKeyPolicy")]
[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected BaseApiController(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected IActionResult OkApiResult<T>(T result)
    {
        return Ok(new ApiResult<T>(result));
    }

    protected IActionResult OkApiResult() => Ok(new ApiResult());

    protected UserContext CurrentUser
    {
        get
        {
            if (_httpContextAccessor.HttpContext == null)
                throw new UnauthorizedAccessException();

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext.User == null)
                throw new UnauthorizedAccessException();

            var user = httpContext.User;

            if (user.Identity is not { IsAuthenticated: true })
                throw new UnauthorizedAccessException();

            var userIdClaim = user.Claims
                .SingleOrDefault(c => c.Type == AuthClaimTypes.UserId);

            if (userIdClaim == null
                || string.IsNullOrEmpty(userIdClaim.Value)
                || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                throw new UnauthorizedAccessException();

            var emailClaim = user.Claims
                .SingleOrDefault(c => c.Type == AuthClaimTypes.Email);

            if (emailClaim == null || string.IsNullOrEmpty(emailClaim.Value))
                throw new UnauthorizedAccessException();
        
            var clientIdClaim = user.Claims
                .SingleOrDefault(c => c.Type == AuthClaimTypes.ClientId);

            Guid? clientId = null;

            if (clientIdClaim != null
                && !string.IsNullOrEmpty(clientIdClaim.Value))
            {
                if (Guid.TryParse(clientIdClaim.Value, out var userClientId))
                    clientId = userClientId;
                else
                    throw new UnauthorizedAccessException();
            }

            return new UserContext(userId, clientId, emailClaim.Value);
        }
    }
}