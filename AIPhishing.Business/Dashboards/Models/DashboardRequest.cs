namespace AIPhishing.Business.Dashboards.Models;

public record DashboardRequest(
    DateTime? StartDate,
    DateTime? EndDate);