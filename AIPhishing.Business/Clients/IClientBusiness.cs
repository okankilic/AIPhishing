using AIPhishing.Business.Clients.Models;
using AIPhishing.Business.Contexts;

namespace AIPhishing.Business.Clients;

public interface IClientBusiness
{
    Task CreateAsync(ClientEditRequest request, UserContext currentUser);
    Task<ClientListResponse> ListAsync(ClientListRequest request, UserContext currentUser);
}