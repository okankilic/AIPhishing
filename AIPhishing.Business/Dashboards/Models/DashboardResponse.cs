namespace AIPhishing.Business.Dashboards.Models;

public record DashboardResponse(
    DashboardHeaderModel Header,
    IDictionary<DateTime, DashboardDailyCountsModel> DailyCounts,
    IDictionary<string, DashboardDepartmentCountsModel> DepartmentCounts);