using AIPhishing.Business.Clients;
using AIPhishing.Business.Clients.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

public class ClientsController : BaseApiController
{
    private readonly IClientBusiness _clientBusiness;

    public ClientsController(
        IHttpContextAccessor httpContextAccessor,
        IClientBusiness clientBusiness)
        : base(httpContextAccessor)
    {
        _clientBusiness = clientBusiness ?? throw new ArgumentNullException(nameof(clientBusiness));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ClientCreateRequest request)
    {
        await _clientBusiness.CreateAsync(request, CurrentUser);

        return OkApiResult();
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] ClientListRequest request)
    {
        var response = await _clientBusiness.ListAsync(request, CurrentUser);

        return OkApiResult(response);
    }
    
    [HttpGet("{clientId:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid clientId)
    {
        var response = await _clientBusiness.GetAsync(clientId, CurrentUser);

        return OkApiResult(response);
    }
    
    [HttpGet("{clientId:guid}/targets")]
    public async Task<IActionResult> ListTargets([FromRoute] Guid clientId, [FromQuery] ClientTargetListRequest request)
    {
        var response = await _clientBusiness.ListTargetsAsync(clientId, request, CurrentUser);

        return OkApiResult(response);
    }

    [HttpPut("{clientId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid clientId, [FromBody] ClientUpdateRequest request)
    {
        await _clientBusiness.UpdateAsync(clientId, request, CurrentUser);

        return OkApiResult();
    }
    
    [HttpPut("{clientId:guid}/user")]
    public async Task<IActionResult> UpdateUser([FromRoute] Guid clientId, [FromBody] ClientUserEditModel request)
    {
        await _clientBusiness.UpdateUserAsync(clientId, request, CurrentUser);

        return OkApiResult();
    }
    
    [HttpPut("{clientId:guid}/targets")]
    public async Task<IActionResult> ImportTargets([FromRoute] Guid clientId, [FromForm] ClientTargetImportRequest request)
    {
        await _clientBusiness.ImportTargetsAsync(clientId, request.File, CurrentUser);

        return OkApiResult();
    }
    
    [HttpDelete("{clientId:guid}/targets/{targetId:guid}")]
    public async Task<IActionResult> ImportTargets([FromRoute] Guid clientId, [FromRoute] Guid targetId)
    {
        await _clientBusiness.DeleteTargetAsync(clientId, targetId, CurrentUser);

        return OkApiResult();
    }
}