namespace AIPhishing.Business.Reports.Models;

public record ReportExportRequest(
    DateTime? StartDate,
    DateTime? EndDate);