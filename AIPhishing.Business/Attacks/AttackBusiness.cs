using System.Globalization;
using AIPhishing.Business.Attacks.Models;
using AIPhishing.Business.Contexts;
using AIPhishing.Business.Extensions;
using AIPhishing.Business.Integrations;
using AIPhishing.Business.Integrations.Models;
using AIPhishing.Business.Managers;
using AIPhishing.Common.Enums;
using AIPhishing.Common.Exceptions;
using AIPhishing.Database;
using AIPhishing.Database.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AIPhishing.Business.Attacks;

public class AttackBusiness : IAttackBusiness
{
    private readonly PhishingDbContext _dbContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<AttackBusiness> _logger;
    private readonly IPhishingAiApiClient _phishingAiApiClient;
    private readonly IConfiguration _configuration;
    private readonly int _replyMinDuration = 2;
    private readonly int _replyMaxDuration = 5;

    public AttackBusiness(
        PhishingDbContext dbContext, 
        IServiceScopeFactory serviceScopeFactory, 
        ILogger<AttackBusiness> logger, 
        IPhishingAiApiClient phishingAiApiClient, 
        IConfiguration configuration)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _phishingAiApiClient = phishingAiApiClient ?? throw new ArgumentNullException(nameof(phishingAiApiClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _replyMinDuration = _configuration.GetValue<int>("ReplyMinDuration", 2);
        _replyMinDuration = _configuration.GetValue<int>("ReplyMaxDuration", 5);
    }

    public async Task<Guid> CreateAsync(AttackCreateRequest request, UserContext currentUser)
    {
        if (string.IsNullOrEmpty(request.Language?.Trim()))
            throw BusinessException.Required(nameof(request.Language));

        if (request.Types == null || request.Types.Length == 0)
        {
            if (string.IsNullOrEmpty(request.From))
                throw BusinessException.Required(nameof(request.From));
            
            if (string.IsNullOrEmpty(request.DisplayName))
                throw BusinessException.Required(nameof(request.DisplayName));
            
            if (string.IsNullOrEmpty(request.Subject))
                throw BusinessException.Required(nameof(request.Subject));
            
            if(string.IsNullOrEmpty(request.BodyTemplate))
                throw BusinessException.Required(nameof(request.BodyTemplate));
        }

        if (request.CsvFile == null || Path.GetExtension(request.CsvFile.FileName) != ".csv")
            throw BusinessException.Required(nameof(request.CsvFile));

        AttackTargetCsvModel[] users;

        try
        {
            await using var stream = request.CsvFile.OpenReadStream();
            using var reader = new StreamReader(stream);
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null
            };
            using var csv = new CsvReader(reader, csvConfiguration);
            users = csv.GetRecords<AttackTargetCsvModel>().ToArray();
        }
        catch (Exception ex)
        {
            throw BusinessException.Invalid(nameof(request.CsvFile));
        }

        if (users.Length == 0)
            throw BusinessException.Invalid(nameof(request.CsvFile));

        await using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            var attack = new Attack
            {
                Id = Guid.NewGuid(),
                ClientId = currentUser.ClientId,
                Language = request.Language,
                State = AttackStateEnum.Created,
                StartTime = request.StartTime,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.Attacks.AddAsync(attack);

            await _dbContext.SaveChangesAsync();

            var fetchEmailContents = true;

            if (string.IsNullOrEmpty(request.BodyTemplate))
            {
                var targetModels = (from user in users
                        let random = new Random()
                        let index = random.Next(request.Types.Length)
                        select new AttackTargetCreateModel(request.Types[index], user.Email, user.FullName))
                    .ToArray();
                
                await CreateTargetsAsync(currentUser.ClientId, attack.Id, targetModels);
            }
            else
            {
                var targetModels = users
                    .Select(u => new AttackTargetCreateModel(null, u.Email, u.FullName))
                    .ToArray();
                
                await CreateTargetsAsync(currentUser.ClientId, attack.Id, targetModels);
                
                var appUrl = _configuration.GetValue<string>("ApiBaseUrl")!;

                var emailModels = (
                    from user in users
                    let emailId = Guid.NewGuid()
                    let clickUrl = $"{appUrl}/api/webhooks/clicked/{emailId}"
                    let body = request.BodyTemplate.BindObjectProperties(new
                    {
                        user.FullName,
                        user.Email,
                        ClickUrl = clickUrl
                    })
                    select new AttackEmailCreateModel(
                        emailId,
                        user.Email,
                        request.From,
                        request.DisplayName,
                        request.Subject,
                        body,
                        attack.StartTime)
                ).ToArray();

                await CreateEmailsAsync(attack.Id, emailModels);

                await UpdateStateAsync(attack.Id, AttackStateEnum.MailsCreated);

                fetchEmailContents = false;
            }
            
            await ts.CommitAsync();

            if (fetchEmailContents)
            {
                _ = Task.Run(async () =>
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    var attackManager = new AttackManager(scope.ServiceProvider, attack.Id);

                    await attackManager.FetchEmailContents();
                });
            }

            return attack.Id;
        }
        catch (Exception)
        {
            await ts.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateStateAsync(Guid id, AttackStateEnum state, string? errorMessage = null)
    {
        var attack = await _dbContext.Attacks.SingleOrDefaultAsync(q => q.Id == id)
                     ?? throw new BusinessException($"{nameof(Attack)} not found. Id: {id}");

        attack.State = state;
        attack.ErrorMessage = errorMessage;
        attack.UpdatedAt = DateTime.UtcNow;

        _dbContext.Attacks.Update(attack);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<AttackViewModel> GetAsync(Guid id, UserContext? currentUser = null)
    {
        var attack = await _dbContext.Attacks
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.Id == id)
                     ?? throw new BusinessException($"{nameof(Attack)} not found. Id: {id}");

        var targets = await _dbContext.AttackTargets
            .Select(q => new
            {
                q.AttackId,
                q.AttackType,
                q.TargetEmail,
                q.TargetFullName,
                q.Succeeded
            })
            .Where(q => q.AttackId == attack.Id)
            .ToArrayAsync();
            
        var targetViewModels = targets
            .Select(target => new AttackTargetViewModel(
                target.AttackType,
                target.TargetEmail, 
                target.TargetFullName, 
                target.Succeeded))
            .ToArray();

        return new AttackViewModel(attack.Id, attack.Language, attack.State, attack.StartTime, targetViewModels);
    }
    
    public async Task CreateTargetsAsync(Guid? clientId, Guid id, AttackTargetCreateModel[] models)
    {
        var targets = models
            .Select(u => new AttackTarget
            {
                AttackId = id,
                AttackType = u.AttackType,
                TargetEmail = u.Email,
                TargetFullName = u.FullName
            })
            .ToArray();

        if (clientId != null)
        {
            var clientTargetEmails = await _dbContext.ClientTargets
                .AsNoTracking()
                .Where(q => q.ClientId == clientId)
                .Select(q => q.Email)
                .ToArrayAsync();

            var filteredTargets = targets
                .Where(q => clientTargetEmails.Contains(q.TargetEmail))
                .ToArray();
            
            await _dbContext.AttackTargets.AddRangeAsync(filteredTargets);
        }
        else
        {
            await _dbContext.AttackTargets.AddRangeAsync(targets);
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateEmailsAsync(Guid id, AttackEmailCreateModel[] models)
    {
        var emails = models
            .Select(q => new AttackEmail
            {
                Id = q.Id,
                AttackId = id,
                State = EmailStateEnum.Created,
                From = q.From,
                DisplayName = q.DisplayName,
                To = q.To,
                Subject = q.Subject,
                Body = q.Body,
                SendAt = q.SendAt,
                CreatedAt = DateTime.UtcNow
            })
            .ToArray();

        await _dbContext.AttackEmails.AddRangeAsync(emails);

        await _dbContext.SaveChangesAsync();
    }

    public async Task EmailOpenedAsync(Guid emailId)
    {
        var email = await _dbContext.AttackEmails.SingleOrDefaultAsync(q => q.Id == emailId && !q.IsOpened);
        
        if (email == null)
            return;
        
        await using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            email.IsOpened = true;
            email.OpenedAt = DateTime.UtcNow;

            _dbContext.AttackEmails.Update(email);

            var target = await _dbContext.AttackTargets
                             .SingleOrDefaultAsync(q => q.AttackId == email.AttackId
                                                        && q.TargetEmail == email.To)
                         ?? throw new BusinessException($"{nameof(AttackTarget)} not found. AttackId: {email.AttackId}, TargetEmail: {email.To}");

            target.Succeeded = true;

            _dbContext.AttackTargets.Update(target);

            await _dbContext.SaveChangesAsync();

            await ts.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occured in {nameof(EmailClickedAsync)}");
            
            await ts.RollbackAsync();
        }
    }

    public async Task EmailClickedAsync(Guid emailId)
    {
        var email = await _dbContext.AttackEmails.SingleOrDefaultAsync(q => q.Id == emailId && !q.IsClicked);
        
        if (email == null)
            return;

        await using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            email.IsClicked = true;
            email.ClickedAt = DateTime.UtcNow;

            _dbContext.AttackEmails.Update(email);

            var target = await _dbContext.AttackTargets
                .SingleOrDefaultAsync(q => q.AttackId == email.AttackId
                                           && q.TargetEmail == email.To)
                ?? throw new BusinessException($"{nameof(AttackTarget)} not found. AttackId: {email.AttackId}, TargetEmail: {email.To}");

            target.Succeeded = true;

            _dbContext.AttackTargets.Update(target);

            await _dbContext.SaveChangesAsync();

            await ts.CommitAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occured in {nameof(EmailClickedAsync)}");
            
            await ts.RollbackAsync();
        }
    }

    public async Task<AttackListResponse> ListAsync(AttackListRequest request, UserContext currentUser)
    {
        if (request == null)
            throw BusinessException.Required(nameof(request));
        
        var count = await _dbContext.Attacks.CountAsync();

        var pageSize = request.PageSize > 0
            ? request.PageSize
            : 10;

        var page = request.CurrentPage > 0
            ? request.CurrentPage
            : 1;
        
        var attacks = await _dbContext.Attacks
            .Where(q => q.ClientId == currentUser.ClientId)
            .Select(q => new
            {
                q.Id,
                q.Language,
                q.State,
                q.CreatedAt
            })
            .OrderByDescending(q => q.CreatedAt)
            .Skip(pageSize * (page - 1))
            .Take(pageSize)
            .ToArrayAsync();

        var targets = await _dbContext.AttackTargets
            .AsNoTracking()
            .GroupBy(q => q.AttackId)
            .Select(q => new
            {
                AttackId = q.Key,
                SuccessCount = q.Count(i => i.Succeeded),
                TargetCount = q.Count()
            })
            .ToArrayAsync();

        var response = (from attack in attacks
                join target in targets on attack.Id equals target.AttackId
                select new AttackListViewModel(attack.Id, attack.Language, attack.State, target.SuccessCount, target.TargetCount, attack.CreatedAt))
            .ToArray();

        return new AttackListResponse(response, count);
    }

    public async Task EmailReplied(AttackEmailRepliedModel model)
    {
        if (model == null)
            throw BusinessException.Required(nameof(model));
        
        var email = await _dbContext.AttackEmails
                        .SingleOrDefaultAsync(q => q.Id == model.EmailId)
                    ?? throw BusinessException.NotFound(nameof(AttackEmail), model.EmailId);
        
        var target = await _dbContext.AttackTargets
                         .SingleOrDefaultAsync(q => q.AttackId == email.AttackId
                                                    && q.TargetEmail == email.To)
                     ?? throw new BusinessException($"{nameof(AttackTarget)} not found. AttackId: {email.AttackId}, TargetEmail: {email.To}");

        await using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            email.IsReplied = true;
            email.RepliedAt = DateTime.UtcNow;

            _dbContext.AttackEmails.Update(email);
            
            target.Succeeded = true;

            _dbContext.AttackTargets.Update(target);

            var reply = new AttackEmailReply
            {
                Id = Guid.NewGuid(),
                AttackEmailId = email.Id,
                Subject = email.Subject,
                Body = model.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _dbContext.AttackEmailReplies.AddAsync(reply);

            await _dbContext.SaveChangesAsync();

            await ts.CommitAsync();
        
            _ = Task.Run(async () =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
            
                var attackBusiness = scope.ServiceProvider.GetRequiredService<IAttackBusiness>();

                try
                {
                    await attackBusiness.ReplyEmailAsync(reply.Id);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An error occured in ReplyEmailAsync");
                }
            });
        }
        catch (Exception)
        {
            await ts.RollbackAsync();
            throw;
        }
    }

    public async Task ReplyEmailAsync(Guid replyEmailId)
    {
        var reply = await _dbContext.AttackEmailReplies
                             .AsNoTracking()
                             .SingleOrDefaultAsync(q => q.Id == replyEmailId)
                         ?? throw BusinessException.NotFound(nameof(AttackEmailReply), replyEmailId);
        
        var email = await _dbContext.AttackEmails
                        .AsNoTracking()
                        .SingleOrDefaultAsync(q => q.Id == reply.AttackEmailId)
                    ?? throw BusinessException.NotFound(nameof(AttackEmail), reply.AttackEmailId);

        var target = await _dbContext.AttackTargets
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.AttackId == email.AttackId
                                                    && q.TargetEmail == email.To)
                     ?? throw BusinessException.NotFound(nameof(AttackTarget), email.To);

        var attack = await _dbContext.Attacks
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.Id == target.AttackId)
                     ?? throw BusinessException.NotFound(nameof(Attack), target.AttackId);
        
        var appUrl = _configuration.GetValue<string>("ApiBaseUrl")!;
        var newEmailId = Guid.NewGuid();
        var linkUrl = $"{appUrl}/api/webhooks/clicked/{newEmailId}";
        
        var response = await _phishingAiApiClient.GetReplyEmailContentAsync(attack.Language, new PhishingAiGetReplyEmailContentRequest(
            target.TargetFullName,
            target.TargetEmail,
            newEmailId,
            linkUrl,
            reply.Subject,
            reply.Body));

        var newEmail = new AttackEmail
        {
            Id = newEmailId,
            AttackId = email.AttackId,
            State = EmailStateEnum.Created,
            From = email.From,
            DisplayName = email.DisplayName,
            To = target.TargetEmail,
            Subject = reply.Subject,
            Body = response,
            CreatedAt = DateTime.UtcNow,
            SendAt = DateTime.UtcNow.AddMinutes(new Random().Next(_replyMinDuration, _replyMaxDuration))
        };

        await _dbContext.AttackEmails.AddAsync(newEmail);

        await _dbContext.SaveChangesAsync();
    }
}