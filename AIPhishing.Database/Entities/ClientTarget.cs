namespace AIPhishing.Database.Entities;

public sealed class ClientTarget
{
    public required Guid Id { get; set; }
    public required Guid ClientId { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required DateTime CreatedAt { get; set; }
    public string? Department { get; set; }
    
    //  navigations
    public Client Client { get; set; }
    public ICollection<Conversation> Conversations { get; set; }
}