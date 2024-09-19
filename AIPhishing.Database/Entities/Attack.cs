using AIPhishing.Common.Enums;

namespace AIPhishing.Database.Entities;

public class Attack
{
    public Guid Id { get; set; }
    public string Language { get; set; } = string.Empty;
    public AttackStateEnum State { get; set; }
    public string? Template { get; set; } = null;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? ErrorMessage { get; set; } = null;
    public DateTime? StartTime { get; set; }
    
    //  Navigations
    public virtual ICollection<AttackTarget> Targets { get; set; }
    public virtual ICollection<AttackEmail> Emails { get; set; }
}