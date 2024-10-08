﻿using AIPhishing.Business.Auth;
using AIPhishing.Business.Auth.Models;
using AIPhishing.WebAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthBusiness _authBusiness;

    public AuthController(
        IHttpContextAccessor httpContextAccessor,
        IAuthBusiness authBusiness)
        : base(httpContextAccessor)
    {
        _authBusiness = authBusiness ?? throw new ArgumentNullException(nameof(authBusiness));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResult<AuthLoginResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
    {
        var response = await _authBusiness.LoginAsync(request);

        return OkApiResult(response);
    }

    [HttpPut("update-password")]
    public async Task<IActionResult> UpdatePassword([FromBody] AuthUpdatePasswordRequest request)
    {
        await _authBusiness.UpdatePasswordAsync(request, CurrentUser);

        return OkApiResult();
    }
}