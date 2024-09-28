namespace AIPhishing.Business.Clients.Models;

public record ClientListViewModel(
    Guid Id,
    string ClientName,
    DateTime CreatedAt);