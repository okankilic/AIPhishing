namespace AIPhishing.Business.Auth.Models;

public record AuthUserResponse(
    string Email,
    Guid? ClientId);