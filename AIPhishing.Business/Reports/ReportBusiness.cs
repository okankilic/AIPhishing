using AIPhishing.Business.Contexts;
using AIPhishing.Business.Reports.Models;
using AIPhishing.Common.Exceptions;
using AIPhishing.Database;
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
            .Where(q => (request.StartDate == null || q.CreatedAt >= request.StartDate)
                        && (request.EndDate == null || q.CreatedAt <= request.EndDate));

        var attackTargets = _dbContext.AttackTargets
            .AsNoTracking();

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking();

        var phishings = from attackEmail in attackEmails
            join attackTarget in attackTargets
                on new { attackEmail.AttackId, Email = attackEmail.To } equals new { attackTarget.AttackId, Email = attackTarget.TargetEmail }
            join clientTarget in clientTargets on attackTarget.TargetEmail equals clientTarget.Email into gj
            from g in gj.DefaultIfEmpty()
            select new
            {
                Department = g != null 
                    ? g.Department 
                    : "N/A",
                Email = attackTarget.TargetEmail,
                FullName = attackTarget.TargetFullName,
                Phished = attackEmail.IsReplied || attackEmail.IsClicked || attackEmail.IsOpened
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
            .Where(q => q.Attack.ClientId == clientId
                        && (request.StartDate == null || q.CreatedAt >= request.StartDate)
                        && (request.EndDate == null || q.CreatedAt <= request.EndDate));

        var attackTargets = _dbContext.AttackTargets
            .AsNoTracking()
            .Where(q => q.Attack.ClientId == clientId);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking()
            .Where(q => q.ClientId == clientId);

        var phishings = from attackEmail in attackEmails
            join attackTarget in attackTargets
                on new { attackEmail.AttackId, Email = attackEmail.To } equals new { attackTarget.AttackId, Email = attackTarget.TargetEmail }
            join clientTarget in clientTargets
                on attackTarget.TargetEmail equals clientTarget.Email
            select new
            {
                ClientTargetId = clientTarget.Id,
                clientTarget.Department,
                clientTarget.Email,
                clientTarget.FullName,
                Phished = attackEmail.IsReplied || attackEmail.IsClicked || attackEmail.IsOpened
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

    private async Task<ReportItemListResponse> GetGodUserItemsAsync(ReportItemListRequest request)
    {
        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;

        var attackEmails = _dbContext.AttackEmails
            .AsNoTracking()
            .Where(q => (request.StartDate == null || q.CreatedAt >= request.StartDate)
                        && (request.EndDate == null || q.CreatedAt <= request.EndDate));

        var attackTargets = _dbContext.AttackTargets
            .AsNoTracking();

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking();

        var phishings = from attackEmail in attackEmails
            join attackTarget in attackTargets
                on new { attackEmail.AttackId, Email = attackEmail.To } equals new { attackTarget.AttackId, Email = attackTarget.TargetEmail }
            join clientTarget in clientTargets on attackTarget.TargetEmail equals clientTarget.Email into gj
            from g in gj.DefaultIfEmpty()
            select new
            {
                Department = g != null 
                    ? g.Department 
                    : "N/A",
                Email = attackTarget.TargetEmail,
                FullName = attackTarget.TargetFullName,
                ScenarioName = attackTarget.AttackType == null
                    ? "Custom"
                    : attackTarget.AttackType,
                SendDate = attackEmail.SentAt,
                Status = attackEmail.SentAt == null
                    ? "Pending"
                    : attackEmail.IsReplied 
                        ? "Replied" 
                        : attackEmail.IsClicked
                            ? "Clicked"
                            : attackEmail.IsOpened
                                ? "Viewed"
                                : "Not Viewed",
                attackEmail.CreatedAt
            };
        
        var totalCount = await phishings.CountAsync();

        var items = await phishings
            .OrderByDescending(q => q.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Select(q => new ReportItemModel(q.Email, q.Department, q.ScenarioName, q.SendDate, q.Status))
            .ToArrayAsync();

        return new ReportItemListResponse(items, totalCount);
    }

    private async Task<ReportItemListResponse> GetClientItemsAsync(Guid clientId, ReportItemListRequest request)
    {
        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;

        var attackEmails = _dbContext.AttackEmails
            .AsNoTracking()
            .Where(q => q.Attack.ClientId == clientId
                && (request.StartDate == null || q.CreatedAt >= request.StartDate)
                && (request.EndDate == null || q.CreatedAt <= request.EndDate)
            );

        var attackTargets = _dbContext.AttackTargets
            .AsNoTracking()
            .Where(q => q.Attack.ClientId == clientId);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking()
            .Where(q => q.ClientId == clientId);

        var phishings = from attackEmail in attackEmails
            join attackTarget in attackTargets
                on new { attackEmail.AttackId, Email = attackEmail.To } equals new { attackTarget.AttackId, Email = attackTarget.TargetEmail }
            join clientTarget in clientTargets
                on attackTarget.TargetEmail equals clientTarget.Email
            select new
            {
                ClientTargetId = clientTarget.Id,
                clientTarget.Department,
                Email = attackTarget.TargetEmail,
                FullName = attackTarget.TargetFullName,
                ScenarioName = attackTarget.AttackType == null
                    ? "Custom"
                    : attackTarget.AttackType,
                SendDate = attackEmail.SentAt,
                Status = attackEmail.SentAt == null
                    ? "Pending"
                    : attackEmail.IsReplied 
                        ? "Replied" 
                        : attackEmail.IsClicked
                            ? "Clicked"
                            : attackEmail.IsOpened
                                ? "Viewed"
                                : "Not Viewed",
                attackEmail.CreatedAt
            };

        var totalCount = await phishings.CountAsync();

        var items = await phishings
            .OrderByDescending(q => q.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .Select(q => new ReportItemModel(q.Email, q.Department, q.ScenarioName, q.SendDate, q.Status))
            .ToArrayAsync();

        return new ReportItemListResponse(items, totalCount);
    }
}