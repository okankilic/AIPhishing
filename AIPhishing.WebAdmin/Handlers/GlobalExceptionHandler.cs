using AIPhishing.Common.Exceptions;
using AIPhishing.WebAdmin.Models;
using Microsoft.AspNetCore.Diagnostics;

namespace AIPhishing.WebAdmin.Handlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger ?? throw new ArgumentNullException(nameof (logger));
    
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogError(ex, "An exception caught by ExceptionMiddleware");
        
        ApiResult result;
        
        switch (ex)
        {
            case UnauthorizedAccessException _:
                context.Response.StatusCode = 401;
                result = new ApiResult(401, ex.Message);
                break;
            case BusinessException exception:
                context.Response.StatusCode = 400;
                result = new ApiResult(400, exception.Message);
                break;
            case IntegrationException exception:
                context.Response.StatusCode = 400;
                result = new ApiResult(999, exception.Message);
                break;
            default:
                context.Response.StatusCode = 500;
                result = new ApiResult(500, "Technical error occured.");
                break;
        }
        
        await context.Response.WriteAsJsonAsync(result, cancellationToken: cancellationToken);

        return true;
    }
}