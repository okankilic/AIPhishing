using AIPhishing.Business.Contexts;
using AIPhishing.Business.Reports.Models;

namespace AIPhishing.Business.Reports;

public interface IReportBusiness
{
    Task<ReportHeaderModel> GetHeaderAsync(UserContext currentUser);
}