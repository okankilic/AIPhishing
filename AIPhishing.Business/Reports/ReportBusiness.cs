using AIPhishing.Business.Contexts;
using AIPhishing.Business.Reports.Models;
using AIPhishing.Common.Exceptions;
using AIPhishing.Database;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace AIPhishing.Business.Reports;

public class ReportBusiness : IReportBusiness
{
    private readonly PhishingDbContext _dbContext;

    public ReportBusiness(
        PhishingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<ReportHeaderModel> GetHeaderAsync(ReportHeaderRequest request, UserContext currentUser)
    {
        if (request == null)
            throw BusinessException.Required(nameof(request));

        if (request.StartDate != null && request.EndDate != null && request.StartDate > request.EndDate)
            throw new BusinessException($"Start date cannot be later than End date");
        
        if (!currentUser.IsGodUser)
        {
            return await GetClientHeaderAsync(currentUser.ClientId!.Value, request);
        }

        return await GetGodUserHeaderAsync(request);
    }

    private async Task<ReportHeaderModel> GetGodUserHeaderAsync(ReportHeaderRequest request)
    {
        var attackEmails = _dbContext.AttackEmails
            .AsNoTracking()
            .Where(q => q.AttackEmailReplyId == null 
                        && (request.StartDate == null || q.CreatedAt >= request.StartDate)
                        && (request.EndDate == null || q.CreatedAt <= request.EndDate));

        var conversations = _dbContext.Conversations
            .AsNoTracking();

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking();

        var phishings = from attackEmail in attackEmails
            join conversation in conversations 
                on attackEmail.ConversationId equals conversation.Id
            join clientTarget in clientTargets 
                on conversation.ClientTargetId equals clientTarget.Id
            select new
            {
                Department = clientTarget.Department != null 
                    ? clientTarget.Department 
                    : "N/A",
                clientTarget.Email,
                clientTarget.FullName,
                Phished = conversation.IsClicked
            };

        var totalPhishings = await phishings.CountAsync();

        var phishedCount = await phishings.CountAsync(q => q.Phished);

        var phishedPercentage = totalPhishings == 0
            ? 0
            : (int)((double)phishedCount / (double)totalPhishings * 100);

        var departments = await phishings
            .GroupBy(q => q.Department)
            .Select(q => new
            {
                Department = q.Key,
                Phished = q.Count(i => i.Phished),
                Total = q.Count()
            })
            .ToArrayAsync();

        departments = departments
            .OrderBy(q => (int)((double)q.Phished / (double)q.Total * 100))
            .ToArray();

        var worst = departments.LastOrDefault();
        var best = departments.FirstOrDefault();

        return new ReportHeaderModel(
            totalPhishings,
            phishedPercentage,
            worst?.Department ?? string.Empty,
            best?.Department ?? string.Empty);
    }

    private async Task<ReportHeaderModel> GetClientHeaderAsync(Guid clientId, ReportHeaderRequest request)
    {
        var attackEmails = _dbContext.AttackEmails
            .AsNoTracking()
            .Where(q => q.Conversation.Attack.ClientId == clientId
                        && q.AttackEmailReplyId == null
                        && (request.StartDate == null || q.CreatedAt >= request.StartDate)
                        && (request.EndDate == null || q.CreatedAt <= request.EndDate));

        var conversations = _dbContext.Conversations
            .AsNoTracking()
            .Where(q => q.Attack.ClientId == clientId);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking()
            .Where(q => q.ClientId == clientId);

        var phishings = from attackEmail in attackEmails
            join conversation in conversations
                on attackEmail.ConversationId equals conversation.Id
            join clientTarget in clientTargets
                on conversation.ClientTargetId equals clientTarget.Id
            select new
            {
                conversation.ClientTargetId,
                Department = clientTarget.Department != null 
                    ? clientTarget.Department 
                    : "N/A",
                clientTarget.Email,
                clientTarget.FullName,
                Phished = conversation.IsClicked
            };

        var totalPhishings = await phishings.CountAsync();

        var phishedCount = await phishings.CountAsync(q => q.Phished);

        var phishedPercentage = totalPhishings == 0
            ? 0
            : (int)((double)phishedCount / (double)totalPhishings * 100);

        var departments = await phishings
            .GroupBy(q => q.Department)
            .Select(q => new
            {
                Department = q.Key,
                Succeeded = q.Count(i => i.Phished),
                Total = q.Count()
            })
            .ToArrayAsync();

        departments = departments
            .OrderBy(q => (int)((double)q.Succeeded / (double)q.Total * 100))
            .ToArray();

        var worst = departments.LastOrDefault();
        var best = departments.FirstOrDefault();

        return new ReportHeaderModel(
            totalPhishings,
            phishedPercentage,
            worst?.Department ?? string.Empty,
            best?.Department ?? string.Empty);
    }

    public async Task<ReportItemListResponse> GetItemsAsync(ReportItemListRequest request, UserContext currentUser)
    {
        if (request == null)
            throw BusinessException.Required(nameof(request));

        if (request.StartDate != null && request.EndDate != null && request.StartDate > request.EndDate)
            throw new BusinessException($"Start date cannot be later than End date");

        if (!currentUser.IsGodUser)
        {
            return await GetClientItemsAsync(currentUser.ClientId!.Value, request);
        }

        return await GetGodUserItemsAsync(request);
    }

    public async Task<Stream> ExportAsync(ReportExportRequest request, UserContext currentUser)
    {
        ReportItemListResponse itemListResponse;
        
        if (!currentUser.IsGodUser)
        {
            itemListResponse = await GetClientItemsAsync(
                currentUser.ClientId!.Value, 
                new ReportItemListRequest(request.StartDate, request.EndDate, 0, 0), 
                true);
        }
        else
        {
            itemListResponse = await GetGodUserItemsAsync(
                new ReportItemListRequest(request.StartDate, request.EndDate, 0, 0), 
                true);
        }

        using var wb = new XLWorkbook();

        var ws = wb.AddWorksheet();

        ws.FirstCell().InsertTable(itemListResponse.Items, "ReportItems", true);

        ws.Columns().AdjustToContents();
        ws.Rows().AdjustToContents();

        var ms = new MemoryStream();
            
        wb.SaveAs(ms);

        return ms;
    }

    private async Task<ReportItemListResponse> GetGodUserItemsAsync(ReportItemListRequest request, bool isExport = false)
    {
        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;

        var attackEmails = _dbContext.AttackEmails
            .AsNoTracking()
            .Where(q => q.AttackEmailReplyId == null 
                        && (request.StartDate == null || q.CreatedAt >= request.StartDate)
                        && (request.EndDate == null || q.CreatedAt <= request.EndDate));

        var conversations = _dbContext.Conversations
            .AsNoTracking();

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking();

        var phishings = from attackEmail in attackEmails
            join conversation in conversations
                on attackEmail.ConversationId equals conversation.Id
            join clientTarget in clientTargets 
                on conversation.ClientTargetId equals clientTarget.Id
            select new
            {
                Department = clientTarget.Department != null 
                    ? clientTarget.Department 
                    : "N/A",
                Email = clientTarget.Email,
                FullName = clientTarget.FullName,
                ScenarioName = conversation.AttackType == null
                    ? "Custom"
                    : conversation.AttackType,
                SendDate = attackEmail.SentAt,
                Status = attackEmail.SentAt == null
                    ? "Pending"
                    : conversation.IsReplied 
                        ? "Replied" 
                        : conversation.IsClicked
                            ? "Clicked"
                            : conversation.IsOpened
                                ? "Viewed"
                                : "Not Viewed",
                attackEmail.CreatedAt
            };
        
        var totalCount = await phishings.CountAsync();

        var items = await phishings
            .OrderByDescending(q => q.CreatedAt)
            .Skip(isExport 
                ? 0 
                : pageSize * (page - 1))
            .Take(isExport 
                ? totalCount 
                : pageSize)
            .Select(q => new ReportItemModel(q.Email, q.Department, q.ScenarioName, q.SendDate, q.Status))
            .ToArrayAsync();

        return new ReportItemListResponse(items, totalCount);
    }

    private async Task<ReportItemListResponse> GetClientItemsAsync(Guid clientId, ReportItemListRequest request, bool isExport = false)
    {
        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;

        var attackEmails = _dbContext.AttackEmails
            .AsNoTracking()
            .Where(q => q.Conversation.Attack.ClientId == clientId
                        && q.AttackEmailReplyId == null
                        && (request.StartDate == null || q.CreatedAt >= request.StartDate)
                        && (request.EndDate == null || q.CreatedAt <= request.EndDate)
            );

        var conversations = _dbContext.Conversations
            .AsNoTracking()
            .Where(q => q.Attack.ClientId == clientId);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking()
            .Where(q => q.ClientId == clientId);

        var phishings = from attackEmail in attackEmails
            join conversation in conversations
                on attackEmail.ConversationId equals conversation.Id
            join clientTarget in clientTargets
                on conversation.ClientTargetId equals clientTarget.Id
            select new
            {
                ClientTargetId = clientTarget.Id,
                clientTarget.Department,
                Email = clientTarget.Email,
                FullName = clientTarget.FullName,
                ScenarioName = conversation.AttackType == null
                    ? "Custom"
                    : conversation.AttackType,
                SendDate = attackEmail.SentAt,
                Status = attackEmail.SentAt == null
                    ? "Pending"
                    : conversation.IsReplied 
                        ? "Replied" 
                        : conversation.IsClicked
                            ? "Clicked"
                            : conversation.IsOpened
                                ? "Viewed"
                                : "Not Viewed",
                attackEmail.CreatedAt
            };

        var totalCount = await phishings.CountAsync();

        var items = await phishings
            .OrderByDescending(q => q.CreatedAt)
            .Skip(isExport 
                ? 0 
                : pageSize * (page - 1))
            .Take(isExport 
                ? totalCount 
                : pageSize)
            .Select(q => new ReportItemModel(q.Email, q.Department, q.ScenarioName, q.SendDate, q.Status))
            .ToArrayAsync();

        return new ReportItemListResponse(items, totalCount);
    }
}