namespace AIPhishing.Database.Entities;

public sealed class Client
{
    public Guid Id { get; set; }
    public required string ClientName { get; set; }
    public required DateTime CreatedAt { get; set; }
    
    //  navigations
    public ICollection<User> Users { get; set; }
    public ICollection<ClientTarget> Targets { get; set; }
    public ICollection<Attack> Attacks { get; set; }
}