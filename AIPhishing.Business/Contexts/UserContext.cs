namespace AIPhishing.Business.Contexts;

public record UserContext(
    Guid Id,
    Guid? ClientId,
    string Email)
{
    public bool IsGodUser => ClientId == null;
}