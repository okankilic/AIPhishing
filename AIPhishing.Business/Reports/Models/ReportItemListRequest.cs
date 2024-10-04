namespace AIPhishing.Business.Reports.Models;

public record ReportItemListRequest(
    DateTime? StartDate,
    DateTime? EndDate,
    int PageSize, 
    int CurrentPage);