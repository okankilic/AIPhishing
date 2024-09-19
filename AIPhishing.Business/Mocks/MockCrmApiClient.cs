using AIPhishing.Business.Integrations;
using AIPhishing.Business.Integrations.Models;

namespace AIPhishing.Business.Mocks;

public class MockCrmApiClient : ICrmApiClient
{
    public async Task<CrmGetUsersResponse> GetUsersAsync(CrmGetUsersRequest request)
    {
        await Task.Delay(2000);

        return new CrmGetUsersResponse([
            // new CrmUser("s.okankilic@gmail.com", "Okan Kılıç"),
            new CrmUser("ilkem.cengiz@gmail.com", "İlkem Onur Cengiz")
        ]);
    }
}