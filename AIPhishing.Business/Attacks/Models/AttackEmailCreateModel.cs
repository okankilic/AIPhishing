namespace AIPhishing.Business.Attacks.Models;

public record AttackEmailCreateModel(
    Guid ConversationId,
    Guid Id, 
    string To, 
    string From, 
    string DisplayName, 
    string Subject, 
    string Body,
    DateTime? SendAt);