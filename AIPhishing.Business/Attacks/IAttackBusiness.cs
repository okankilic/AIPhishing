using AIPhishing.Business.Attacks.Models;
using AIPhishing.Common.Enums;

namespace AIPhishing.Business.Attacks;

public interface IAttackBusiness
{
    Task<Guid> CreateAsync(AttackCreateRequest request);
    Task UpdateStateAsync(Guid id, AttackStateEnum state, string? errorMessage = null);
    Task<AttackViewModel> GetAsync(Guid id);
    Task CreateTargetsAsync(Guid id, AttackTargetCreateModel[] models);
    Task CreateEmailsAsync(Guid id, AttackEmailCreateModel[] models);
    Task EmailOpenedAsync(Guid emailId);
    Task EmailClickedAsync(Guid emailId);
    Task<AttackListResponse> ListAsync(AttackListRequest request);
    Task EmailReplied(AttackEmailRepliedModel model);
    Task ReplyEmailAsync(Guid replyEmailId);
}