using AIPhishing.Business.Contexts;
using AIPhishing.Business.Reports.Models;

namespace AIPhishing.Business.Reports;

public interface IReportBusiness
{
    Task<ReportHeaderModel> GetHeaderAsync(ReportHeaderRequest request, UserContext currentUser);
    Task<ReportItemListResponse> GetItemsAsync(ReportItemListRequest request, UserContext currentUser);
    Task<Stream> ExportAsync(ReportExportRequest request, UserContext currentUser);
}