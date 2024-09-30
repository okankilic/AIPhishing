namespace AIPhishing.Business.Clients.Models;

public record ClientTargetListResponse(ClientTargetListViewModel[] Targets, int TotalCount);