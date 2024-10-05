using AIPhishing.Business.Contexts;
using AIPhishing.Business.Dashboards.Models;

namespace AIPhishing.Business.Dashboards;

public interface IDashboardBusiness
{
    Task<DashboardResponse> GetAsync(DashboardRequest request, UserContext currentUser);
}