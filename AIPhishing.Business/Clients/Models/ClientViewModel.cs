namespace AIPhishing.Business.Clients.Models;

public record ClientViewModel(
    Guid Id,
    string ClientName,
    ClientUserViewModel User);