using AIPhishing.Business.Attacks.Models;
using AIPhishing.Business.Contexts;
using AIPhishing.Common.Enums;

namespace AIPhishing.Business.Attacks;

public interface IAttackBusiness
{
    Task<Guid> CreateAsync(AttackCreateRequest request, UserContext currentUser);
    Task UpdateStateAsync(Guid id, AttackStateEnum state, string? errorMessage = null);
    Task<AttackViewModel> GetAsync(Guid id, UserContext? currentUser = null);
    Task CreateTargetsAsync(Guid? clientId, Guid id, AttackTargetCreateModel[] models);
    Task CreateEmailsAsync(Guid id, AttackEmailCreateModel[] models);
    Task EmailOpenedAsync(Guid emailId);
    Task EmailClickedAsync(Guid emailId);
    Task<AttackListResponse> ListAsync(AttackListRequest request, UserContext currentUser);
    Task EmailReplied(AttackEmailRepliedModel model);
    Task ReplyEmailAsync(Guid replyEmailId);
}