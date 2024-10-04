using AIPhishing.Business.Contexts;
using AIPhishing.Business.Reports.Models;
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

    public async Task<ReportHeaderModel> GetHeaderAsync(UserContext currentUser)
    {
        if (!currentUser.IsGodUser)
        {
            return await GetClientHeaderAsync(currentUser.ClientId!.Value);
        }

        var targets = _dbContext.AttackTargets
            .AsNoTracking();
        
        var totalPhishings = await targets.CountAsync();

        var phishedCount = await targets.CountAsync(q => q.Succeeded);

        var phishedPercentage = totalPhishings == 0 
            ? 0 
            : (int)((double)phishedCount / (double)totalPhishings * 100);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking();

        var departments = await (from target in targets
                join clientTarget in clientTargets on target.TargetEmail equals clientTarget.Email into gj
                from g in gj.DefaultIfEmpty()
                select new
                {
                    Department = g != null ? g.Department : string.Empty,
                    Email = target.TargetEmail,
                    FullName = target.TargetFullName,
                    target.AttackId,
                    target.Succeeded
                })
            .GroupBy(q => q.Department)
            .Select(q => new
            {
                Department = q.Key,
                Succeeded = q.Count(i => i.Succeeded),
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

    private async Task<ReportHeaderModel> GetClientHeaderAsync(Guid clientId)
    {
        var targets = _dbContext.AttackTargets
            .AsNoTracking()
            .Where(q => q.Attack.ClientId == clientId);
        
        var totalPhishings = await targets.CountAsync();

        var phishedCount = await targets.CountAsync(q => q.Succeeded);

        var phishedPercentage = totalPhishings == 0 
            ? 0 
            : (int)((double)phishedCount / (double)totalPhishings * 100);

        var clientTargets = _dbContext.ClientTargets
            .AsNoTracking()
            .Where(q => q.ClientId == clientId);

        var departments = await (from target in targets
                join clientTarget in clientTargets on target.TargetEmail equals clientTarget.Email
                select new
                {
                    ClientTargetId = clientTarget.Id,
                    clientTarget.Department,
                    clientTarget.Email,
                    clientTarget.FullName,
                    target.AttackId,
                    target.Succeeded
                })
            .GroupBy(q => q.Department)
            .Select(q => new
            {
                Department = q.Key,
                Succeeded = q.Count(i => i.Succeeded),
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
}