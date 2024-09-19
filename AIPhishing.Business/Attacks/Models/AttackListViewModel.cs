using AIPhishing.Common.Enums;

namespace AIPhishing.Business.Attacks.Models;

public record AttackListViewModel(
    Guid Id,
    string Language,
    AttackStateEnum State,
    int SuccessCount,
    int TargetCount,
    DateTime CreatedAt)
{
    public double SuccessRate => TargetCount == 0 ? 0 : (SuccessCount / TargetCount) * 100;
}