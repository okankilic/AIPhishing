namespace AIPhishing.Database.Entities;

public sealed class AttackEmailReply
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid AttackEmailId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
    
    //  navigations
    public Conversation Conversation { get; set; }
    public AttackEmail AttackEmail { get; set; }
}