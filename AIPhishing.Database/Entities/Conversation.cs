namespace AIPhishing.Database.Entities;

public sealed class Conversation
{
    public required Guid Id { get; set; }
    public required Guid ClientTargetId { get; set; }
    public required Guid AttackId { get; set; }
    public string? AttackType { get; set; }
    public required string Sender { get; set; }
    public required string Subject { get; set; }
    public required bool IsOpened { get; set; }
    public required bool IsClicked { get; set; }
    public required bool IsReplied { get; set; }
    
    //  navigations
    public Attack Attack { get; set; }
    public ClientTarget ClientTarget { get; set; }
    public ICollection<AttackEmail> AttackEmails { get; set; }
    public ICollection<AttackEmailReply> AttackEmailReplies { get; set; }
}