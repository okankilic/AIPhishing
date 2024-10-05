namespace AIPhishing.Business.Dashboards.Models;

public record DashboardHeaderModel(
    long TotalEmailsSent,
    double PhishedRatePercentage,
    double EngagementRatePercentage);