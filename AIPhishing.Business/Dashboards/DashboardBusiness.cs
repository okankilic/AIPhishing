using AIPhishing.Business.Contexts;
using AIPhishing.Business.Dashboards.Models;
using AIPhishing.Common.Exceptions;
using AIPhishing.Database;
using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIPhishing.Business.Dashboards;

public class DashboardBusiness : IDashboardBusiness
{
    private readonly PhishingDbContext _dbContext;

    public DashboardBusiness(
        PhishingDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<DashboardResponse> GetAsync(DashboardRequest request, UserContext currentUser)
    {
        if (request == null)
            throw BusinessException.Required(nameof(request));
        
        if (request.StartDate != null && request.EndDate != null && request.StartDate > request.EndDate)
            throw new BusinessException($"Start date cannot be later than End date");
        
        if (!currentUser.IsGodUser)
        {
            return await ReturnClientHeaderAsync(currentUser.ClientId!.Value, request);
        }

        return await ReturnGodUserHeaderAsync(request);
    }
    
    private async Task<DashboardResponse> ReturnGodUserHeaderAsync(DashboardRequest request)
    {
        var oldestClient = await _dbContext.Clients
            .AsNoTracking()
            .OrderBy(q => q.CreatedAt)
            .FirstAsync();
        
        var sentEmails = _dbContext.AttackEmails
            .Where(q => q.SentAt != null
                        && (request.StartDate == null || q.SentAt >= request.StartDate)
                        && (request.EndDate == null || q.SentAt <= request.EndDate));

        var totalEmailsSent = await sentEmails.LongCountAsync();

        var phishedCount = await sentEmails.LongCountAsync(q => q.IsClicked);

        var phishedRate = totalEmailsSent == 0
            ? 0
            : Math.Round((double)phishedCount / (double)totalEmailsSent * 100, 1);
            
        var engagementCount = await sentEmails.LongCountAsync(q => q.IsReplied);
            
        var engagementRate = totalEmailsSent == 0
            ? 0
            : Math.Round((double)engagementCount / (double)totalEmailsSent * 100, 1);

        var header = new DashboardHeaderModel(totalEmailsSent, phishedRate, engagementRate);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking();

        var emailViaDepartmens = from email in sentEmails
            join clientTarget in clientTargets on email.To equals clientTarget.Email into gj
            from g in gj.DefaultIfEmpty()
            select new
            {
                Department = g != null 
                    ? g.Department
                    : "N/A",
                email.Id,
                email.IsReplied,
                email.IsClicked,
                email.IsOpened,
                email.SentAt
            };

        var dailyCounts = await emailViaDepartmens
            .GroupBy(q => q.SentAt!.Value.Date)
            .Select(q => new
            {
                Date = q.Key,
                Phished = q.LongCount(i => i.IsReplied || i.IsClicked),
                Engagement = q.LongCount(i => i.IsReplied || i.IsClicked || i.IsOpened)
            })
            .ToDictionaryAsync(
                q => q.Date,
                q => new DashboardDailyCountsModel(q.Phished, q.Engagement));
        
        var startDate = oldestClient.CreatedAt.Date;
        if (request.StartDate.HasValue && request.StartDate.Value.Date > startDate)
            startDate = request.StartDate.Value.Date;

        var endDate = DateTime.UtcNow.Date;
        if (request.EndDate.HasValue && request.EndDate.Value.Date < endDate)
            endDate = request.EndDate.Value.Date;

        var rangeDailyCounts = new Dictionary<DateTime, DashboardDailyCountsModel>();

        while (startDate <= endDate)
        {
            rangeDailyCounts.Add(startDate, 
                dailyCounts.TryGetValue(startDate, out var value) 
                    ? value 
                    : new DashboardDailyCountsModel(0, 0));

            startDate = startDate.AddDays(1);
        }
        
        var departmentCounts = await emailViaDepartmens
            .GroupBy(q => q.Department)
            .Select(q => new
            {
                Department = q.Key,
                Phished = q.LongCount(i => i.IsReplied || i.IsClicked),
                Engagement = q.LongCount(i => i.IsReplied || i.IsClicked || i.IsOpened)
            })
            .ToDictionaryAsync(
                q => q.Department,
                q => new DashboardDepartmentCountsModel(q.Phished, q.Engagement));

        return new DashboardResponse(header, dailyCounts, departmentCounts);
    }

    private async Task<DashboardResponse> ReturnClientHeaderAsync(Guid clientId, DashboardRequest request)
    {
        var client = await _dbContext.Clients
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.Id == clientId)
                     ?? throw BusinessException.NotFound(nameof(Client), clientId);
        
        var sentEmails = _dbContext.AttackEmails
            .Where(q => q.Conversation.Attack.ClientId == clientId
                        && q.SentAt != null
                        && (request.StartDate == null || q.SentAt >= request.StartDate)
                        && (request.EndDate == null || q.SentAt <= request.EndDate));

        var totalEmailsSent = await sentEmails.LongCountAsync();

        var phishedCount = await sentEmails.LongCountAsync(q => q.IsClicked);

        var phishedRate = totalEmailsSent == 0
            ? 0
            : Math.Round((double)phishedCount / (double)totalEmailsSent * 100, 1);
            
        var engagementCount = await sentEmails.LongCountAsync(q => q.IsReplied);
            
        var engagementRate = totalEmailsSent == 0
            ? 0
            : Math.Round((double)engagementCount / (double)totalEmailsSent * 100, 1);
        
        var header = new DashboardHeaderModel(totalEmailsSent, phishedRate, engagementRate);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking()
            .Where(q => q.ClientId == clientId);

        var emailViaDepartmens = from email in sentEmails
            join clientTarget in clientTargets on email.To equals clientTarget.Email
            select new
            {
                clientTarget.Department,
                email.Id,
                email.IsReplied,
                email.IsClicked,
                email.IsOpened,
                email.SentAt
            };

        var dailyCounts = await emailViaDepartmens
            .GroupBy(q => q.SentAt!.Value.Date)
            .Select(q => new
            {
                Date = q.Key,
                Phished = q.LongCount(i => i.IsReplied || i.IsClicked),
                Engagement = q.LongCount(i => i.IsReplied || i.IsClicked || i.IsOpened)
            })
            .ToDictionaryAsync(
                q => q.Date,
                q => new DashboardDailyCountsModel(q.Phished, q.Engagement));

        var startDate = client.CreatedAt.Date;
        if (request.StartDate.HasValue && request.StartDate.Value.Date > startDate)
            startDate = request.StartDate.Value.Date;

        var endDate = DateTime.UtcNow.Date;
        if (request.EndDate.HasValue && request.EndDate.Value.Date < endDate)
            endDate = request.EndDate.Value.Date;

        var rangeDailyCounts = new Dictionary<DateTime, DashboardDailyCountsModel>();

        while (startDate <= endDate)
        {
            rangeDailyCounts.Add(startDate, 
                dailyCounts.TryGetValue(startDate, out var value) 
                    ? value 
                    : new DashboardDailyCountsModel(0, 0));

            startDate = startDate.AddDays(1);
        }
        
        var departmentCounts = await emailViaDepartmens
            .GroupBy(q => q.Department)
            .Select(q => new
            {
                Department = q.Key,
                Phished = q.LongCount(i => i.IsReplied || i.IsClicked),
                Engagement = q.LongCount(i => i.IsReplied || i.IsClicked || i.IsOpened)
            })
            .ToDictionaryAsync(
                q => q.Department,
                q => new DashboardDepartmentCountsModel(q.Phished, q.Engagement));

        return new DashboardResponse(header, rangeDailyCounts, departmentCounts);
    }
}