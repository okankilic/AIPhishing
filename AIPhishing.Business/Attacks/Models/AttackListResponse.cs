namespace AIPhishing.Business.Attacks.Models;

public record AttackListResponse(AttackListViewModel[] Attacks, int TotalCount);