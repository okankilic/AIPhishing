namespace AIPhishing.Business.Attacks.Models;

public record ConversationViewModel(
    Guid Id,
    Guid ClientTargetId,
    string? AttackType,
    string Sender,
    string Subject,
    string Email,
    string FullName,
    bool IsOpened,
    bool IsClicked,
    bool IsReplied);