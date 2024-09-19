using AIPhishing.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.Web.Controllers;

[Authorize(Policy = "ApiKeyPolicy")]
[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult OkApiResult<T>(T result)
    {
        return Ok(new ApiResult<T>(result));
    }

    protected IActionResult OkApiResult() => Ok(new ApiResult());
}