namespace AIPhishing.Database.Entities;

public class AttackTarget
{
    public Guid AttackId { get; set; }
    public string? AttackType { get; set; }
    public string TargetEmail { get; set; } = string.Empty;
    public string TargetFullName { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    
    //  navigations
    public virtual Attack Attack { get; set; }
}