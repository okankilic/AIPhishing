namespace AIPhishing.Business.Clients.Models;

public record ClientListResponse(ClientListViewModel[] Attacks, int TotalCount);