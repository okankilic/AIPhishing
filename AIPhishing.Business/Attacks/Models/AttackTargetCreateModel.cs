using AIPhishing.Common.Enums;

namespace AIPhishing.Business.Attacks.Models;

public record AttackTargetCreateModel(AttackTypeEnum? AttackType, string Email, string FullName);