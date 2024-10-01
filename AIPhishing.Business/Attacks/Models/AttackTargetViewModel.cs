namespace AIPhishing.Business.Attacks.Models;

public record AttackTargetViewModel(
    string? AttackType,
    string Email,
    string FullName,
    bool Succeeded);