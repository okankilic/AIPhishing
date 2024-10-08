﻿using AIPhishing.Business.Attacks;
using AIPhishing.Business.Attacks.Models;
using AIPhishing.Business.Managers;
using AIPhishing.WebAdmin.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

public class AttacksController : BaseApiController
{
    private readonly IAttackBusiness _attackBusiness;
    
    public AttacksController(
        IHttpContextAccessor httpContextAccessor,
        IAttackBusiness attackBusiness)
        : base(httpContextAccessor)
    {
        _attackBusiness = attackBusiness ?? throw new ArgumentNullException(nameof(attackBusiness));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResult<AttackListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] AttackListRequest request)
    {
        var response = await _attackBusiness.ListAsync(request, CurrentUser);

        return OkApiResult(response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<AttackViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var response = await _attackBusiness.GetAsync(id, CurrentUser);

        return OkApiResult(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromForm] AttackCreateRequest request)
    {
        var response = await _attackBusiness.CreateAsync(request, CurrentUser);
        
        return OkApiResult(response);
    }
    
    [HttpPut("fetch-email-content/{id:guid}")]
    [ProducesResponseType(typeof(ApiResult<AttackViewModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> FetchEmailContent([FromRoute] Guid id, [FromServices] IServiceScopeFactory serviceScopeFactory)
    {
        using var scope = serviceScopeFactory.CreateScope();
        
        var manager = new AttackManager(scope.ServiceProvider, id);

        await manager.FetchEmailContents();

        return OkApiResult();
    }
}