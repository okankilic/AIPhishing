using AIPhishing.Common.Enums;

namespace AIPhishing.Database.Entities;

public class AttackEmail
{
    public Guid Id { get; set; }
    public Guid AttackId { get; set; }
    public EmailStateEnum State { get; set; }
    public string From { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SentAt { get; set; }
    public bool IsOpened { get; set; }
    public DateTime? OpenedAt { get; set; }
    public bool IsClicked { get; set; }
    public DateTime? ClickedAt { get; set; }
    public int TryCount { get; set; }
    public string? ErrorMessage { get; set; } = null;
    public DateTime? SendAt { get; set; }
    public bool IsReplied { get; set; }
    public DateTime? RepliedAt { get; set; }
    
    //  navigations
    public virtual Attack Attack { get; set; }
    public virtual ICollection<AttackEmailReply> Replies { get; set; }
}