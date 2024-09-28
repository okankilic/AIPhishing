using AIPhishing.Business.Clients;
using AIPhishing.Business.Clients.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.Web.Controllers;

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
    public async Task<IActionResult> Create([FromBody] ClientEditRequest request)
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
}