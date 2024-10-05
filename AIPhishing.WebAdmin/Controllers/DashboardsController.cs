using AIPhishing.Business.Dashboards;
using AIPhishing.Business.Dashboards.Models;
using AIPhishing.WebAdmin.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

public class DashboardsController : BaseApiController
{
    private readonly IDashboardBusiness _dashboardBusiness;
    
    public DashboardsController(
        IHttpContextAccessor httpContextAccessor, 
        IDashboardBusiness dashboardBusiness) 
        : base(httpContextAccessor)
    {
        _dashboardBusiness = dashboardBusiness ?? throw new ArgumentNullException(nameof(dashboardBusiness));
    }

    [HttpGet]
    [ProducesResponseType<ApiResult<DashboardResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHeader([FromQuery] DashboardRequest request)
    {
        var response = await _dashboardBusiness.GetAsync(request, CurrentUser);

        return OkApiResult(response);
    }
}