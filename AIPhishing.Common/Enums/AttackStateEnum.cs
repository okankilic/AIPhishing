namespace AIPhishing.Common.Enums;

public enum AttackStateEnum
{
    Created,
    // FetchingTargets,
    // TargetsCreated,
    FetchingMailContent,
    MailContentFetched,
    CreatingMails,
    MailsCreated,
    Failed
}