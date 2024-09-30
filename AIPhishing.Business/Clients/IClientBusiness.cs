using AIPhishing.Business.Clients.Models;
using AIPhishing.Business.Contexts;
using Microsoft.AspNetCore.Http;

namespace AIPhishing.Business.Clients;

public interface IClientBusiness
{
    Task CreateAsync(ClientCreateRequest request, UserContext currentUser);
    Task<ClientListResponse> ListAsync(ClientListRequest request, UserContext currentUser);
    Task<ClientTargetListResponse> ListTargetsAsync(Guid clientId, ClientTargetListRequest request, UserContext currentUser);
    Task<ClientViewModel> GetAsync(Guid clientId, UserContext currentUser);
    Task UpdateAsync(Guid clientId, ClientUpdateRequest request, UserContext currentUser);
    Task UpdateUserAsync(Guid clientId, ClientUserEditModel request, UserContext currentUser);
    Task ImportTargetsAsync(Guid clientId, IFormFile file, UserContext currentUser);
    Task DeleteTargetAsync(Guid clientId, Guid targetId, UserContext currentUser);
}