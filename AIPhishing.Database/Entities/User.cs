namespace AIPhishing.Database.Entities;

public sealed class User
{
    public required Guid Id { get; set; }
    public Guid? ClientId { get; set; }
    public required string Email { get; set; } = string.Empty;
    public required string Password { get; set; } = string.Empty;
    public required DateTime CreatedAt { get; set; }
    
    //  navigations
    public Client Client { get; set; }
}