using AIPhishing.Common.Enums;

namespace AIPhishing.Database.Entities;

public sealed class Attack
{
    public required Guid Id { get; set; }
    public Guid? ClientId { get; set; }
    public required string Language { get; set; }
    public required AttackStateEnum State { get; set; }
    public string? Template { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? StartTime { get; set; }
    
    //  Navigations
    public ICollection<Conversation> Conversations { get; set; }
    public Client Client { get; set; }
}