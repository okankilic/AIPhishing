using AIPhishing.Common.Enums;

namespace AIPhishing.Database.Entities;

public sealed class AttackEmail
{
    public required Guid Id { get; set; }
    public required Guid ConversationId { get; set; }
    public Guid? AttackEmailReplyId { get; set; }
    public required EmailStateEnum State { get; set; }
    public required string From { get; set; } = string.Empty;
    public required string DisplayName { get; set; } = string.Empty;
    public required string To { get; set; } = string.Empty;
    public required string Subject { get; set; } = string.Empty;
    public required string Body { get; set; } = string.Empty;
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public bool IsOpened { get; set; }
    public DateTime? OpenedAt { get; set; }
    public bool IsClicked { get; set; }
    public DateTime? ClickedAt { get; set; }
    public required int TryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? SendAt { get; set; }
    public bool IsReplied { get; set; }
    public DateTime? RepliedAt { get; set; }
    
    //  navigations
    public Conversation Conversation { get; set; }
    public ICollection<AttackEmailReply> Replies { get; set; }
    public AttackEmailReply AttackEmailReply { get; set; }
}