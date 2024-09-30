namespace AIPhishing.Business.Clients.Models;

public record ClientListResponse(ClientListViewModel[] Clients, int TotalCount);