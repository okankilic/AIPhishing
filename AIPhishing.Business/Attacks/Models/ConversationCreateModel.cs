namespace AIPhishing.Business.Attacks.Models;

public record ConversationCreateModel( 
    string? AttackType, 
    string Email, 
    string FullName,
    string Sender,
    string Subject);