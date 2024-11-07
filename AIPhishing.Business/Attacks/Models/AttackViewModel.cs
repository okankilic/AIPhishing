using AIPhishing.Common.Enums;

namespace AIPhishing.Business.Attacks.Models;

public record AttackViewModel(
    Guid Id,
    string Language,
    AttackStateEnum State,
    DateTime? StartTime,
    ConversationViewModel[] Conversations)
{
    public double SuccessRate => Conversations.Length == 0 ? 0 : (Conversations.Count(t => t.IsOpened || t.IsClicked || t.IsReplied) / Conversations.Length) * 100;
}