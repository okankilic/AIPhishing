using AIPhishing.Business.Integrations.Models;

namespace AIPhishing.Business.Integrations;

public interface ICrmApiClient
{
    Task<CrmGetUsersResponse> GetUsersAsync(CrmGetUsersRequest request);
}