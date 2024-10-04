namespace AIPhishing.Business.Reports.Models;

public record ReportHeaderRequest(
    DateTime? StartDate,
    DateTime? EndDate);