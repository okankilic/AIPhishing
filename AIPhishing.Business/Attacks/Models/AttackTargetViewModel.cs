using AIPhishing.Common.Enums;

namespace AIPhishing.Business.Attacks.Models;

public record AttackTargetViewModel(
    AttackTypeEnum? AttackType,
    string Email,
    string FullName,
    bool Succeeded);