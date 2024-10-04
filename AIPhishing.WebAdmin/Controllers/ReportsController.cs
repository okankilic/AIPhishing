using AIPhishing.Business.Reports;
using AIPhishing.Business.Reports.Models;
using Microsoft.AspNetCore.Mvc;

namespace AIPhishing.WebAdmin.Controllers;

public class ReportsController : BaseApiController
{
    private readonly IReportBusiness _reportBusiness;
    
    public ReportsController(
        IHttpContextAccessor httpContextAccessor,
        IReportBusiness reportBusiness)
        : base(httpContextAccessor)
    {
        _reportBusiness = reportBusiness ?? throw new ArgumentNullException(nameof(reportBusiness));
    }
    
    [HttpGet("header")]
    public async Task<IActionResult> GetHeader()
    {
        var response = await _reportBusiness.GetHeaderAsync(CurrentUser);

        return OkApiResult(response);
    }
}