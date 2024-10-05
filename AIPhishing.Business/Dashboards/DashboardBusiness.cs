using AIPhishing.Business.Contexts;
using AIPhishing.Business.Dashboards.Models;
using AIPhishing.Common.Exceptions;
using AIPhishing.Database;
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
        var sentEmails = _dbContext.AttackEmails
            .Where(q => q.SentAt != null
                        && (request.StartDate == null || q.SentAt >= request.StartDate)
                        && (request.EndDate == null || q.SentAt <= request.EndDate));

        var totalEmailsSent = await sentEmails.LongCountAsync();

        var phishedCount = await sentEmails.LongCountAsync(q => q.IsReplied || q.IsClicked);

        var phishedRate = totalEmailsSent == 0
            ? 0
            : Math.Round((double)phishedCount / (double)totalEmailsSent * 100, 1);
            
        var engagementCount = await sentEmails.LongCountAsync(q => q.IsReplied || q.IsClicked || q.IsOpened);
            
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
        var sentEmails = _dbContext.AttackEmails
            .Where(q => q.Attack.ClientId == clientId
                        && q.SentAt != null
                        && (request.StartDate == null || q.SentAt >= request.StartDate)
                        && (request.EndDate == null || q.SentAt <= request.EndDate));

        var totalEmailsSent = await sentEmails.LongCountAsync();

        var phishedCount = await sentEmails.LongCountAsync(q => q.IsReplied || q.IsClicked);

        var phishedRate = totalEmailsSent == 0
            ? 0
            : Math.Round((double)phishedCount / (double)totalEmailsSent * 100, 1);
            
        var engagementCount = await sentEmails.LongCountAsync(q => q.IsReplied || q.IsClicked || q.IsOpened);
            
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
}