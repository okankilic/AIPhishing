using AIPhishing.Common.Enums;

namespace AIPhishing.Business.Attacks.Models;

public record AttackViewModel(
    Guid Id,
    string Language,
    AttackStateEnum State,
    DateTime? StartTime,
    AttackTargetViewModel[] Targets)
{
    public double SuccessRate => Targets.Length == 0 ? 0 : (Targets.Count(t => t.Succeeded) / Targets.Length) * 100;
}