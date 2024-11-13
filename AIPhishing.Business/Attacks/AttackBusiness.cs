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
        if (!currentUser.ClientId.HasValue)
            throw new BusinessException($"God users cannot start an attack.");
        
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

        ConversationCsvModel[] users;

        try
        {
            await using var stream = request.CsvFile.OpenReadStream();
            using var reader = new StreamReader(stream);
            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null
            };
            using var csv = new CsvReader(reader, csvConfiguration);
            users = csv.GetRecords<ConversationCsvModel>().ToArray();
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
                var conversationModels = (from user in users
                        let random = new Random()
                        let index = random.Next(request.Types.Length)
                        select new ConversationCreateModel(
                            request.Types[index], 
                            user.Email, 
                            user.FullName,
                            string.Empty,
                            string.Empty))
                    .ToArray();
                
                await CreateConversationsAsync(currentUser.ClientId.Value, attack.Id, conversationModels);
            }
            else
            {
                var conversationModels = users
                    .Select(u => new ConversationCreateModel(
                        null, 
                        u.Email, 
                        u.FullName,
                        request.From,
                        request.Subject))
                    .ToArray();
                
                var conversations = await CreateConversationsAsync(currentUser.ClientId.Value, attack.Id, conversationModels);
                
                var appUrl = _configuration.GetValue<string>("ApiBaseUrl")!;

                var emailModels = (
                    from conversation in conversations
                    let emailId = Guid.NewGuid()
                    let clickUrl = $"{appUrl}/api/webhooks/clicked/{emailId}"
                    let body = request.BodyTemplate.BindObjectProperties(new
                    {
                        conversation.FullName,
                        conversation.Email,
                        ClickUrl = clickUrl
                    })
                    select new AttackEmailCreateModel(
                        conversation.Id,
                        emailId,
                        conversation.Email,
                        conversation.Sender,
                        conversation.Sender,
                        conversation.Subject,
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

        var conversations = await _dbContext.Conversations
            .Select(q => new
            {
                q.Id,
                q.ClientTargetId,
                q.AttackId,
                q.AttackType,
                q.ClientTarget.Email,
                q.ClientTarget.FullName,
                q.Sender,
                q.Subject,
                q.IsOpened,
                q.IsClicked,
                q.IsReplied
            })
            .Where(q => q.AttackId == attack.Id)
            .ToArrayAsync();
            
        var targetViewModels = conversations
            .Select(c => new ConversationViewModel(
                c.Id,
                c.ClientTargetId,
                c.AttackType,
                c.Sender,
                c.Subject,
                c.Email, 
                c.FullName, 
                c.IsOpened,
                c.IsClicked,
                c.IsReplied))
            .ToArray();

        return new AttackViewModel(attack.Id, attack.Language, attack.State, attack.StartTime, targetViewModels);
    }
    
    private async Task<ConversationViewModel[]> CreateConversationsAsync(Guid clientId, Guid attackId, ConversationCreateModel[] models)
    {
        var clientTargetEmails = await _dbContext.ClientTargets
            .AsNoTracking()
            .Where(q => q.ClientId == clientId)
            .Select(q => new
            {
                q.Id,
                q.Email,
                q.FullName
            })
            .ToArrayAsync();
        
        var conversationViewModels = models
            .Join(clientTargetEmails, 
                model => model.Email, 
                clientTarget => clientTarget.Email,
                (model, clientTarget) => new ConversationViewModel(
                    Guid.NewGuid(),
                    clientTarget.Id,
                    model.AttackType,
                    model.Sender,
                    model.Subject,
                    clientTarget.Email,
                    clientTarget.FullName,
                    false,
                    false,
                    false))
            .ToArray();

        var conversations = conversationViewModels
            .Select(q => new Conversation
            {
                Id = q.Id,
                ClientTargetId = q.ClientTargetId,
                AttackId = attackId,
                AttackType = q.AttackType,
                Sender = q.Sender,
                Subject = q.Subject,
                IsOpened = q.IsOpened,
                IsClicked = q.IsClicked,
                IsReplied = q.IsReplied
            })
            .ToArray();
            
        await _dbContext.Conversations.AddRangeAsync(conversations);

        await _dbContext.SaveChangesAsync();

        return conversationViewModels;
    }

    public async Task CreateEmailsAsync(Guid id, AttackEmailCreateModel[] models)
    {
        var emails = models
            .Select(q => new AttackEmail
            {
                Id = q.Id,
                ConversationId = q.ConversationId,
                State = EmailStateEnum.Created,
                From = q.From,
                DisplayName = q.DisplayName,
                To = q.To,
                Subject = q.Subject,
                Body = q.Body,
                SendAt = q.SendAt,
                TryCount = 0,
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

            var conversation = await _dbContext.Conversations
                             .SingleOrDefaultAsync(q => q.Id == email.ConversationId)
                         ?? throw BusinessException.NotFound(nameof(Conversation), email.ConversationId);

            conversation.IsOpened = true;

            _dbContext.Conversations.Update(conversation);

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
        var email = await _dbContext.AttackEmails.SingleOrDefaultAsync(q => q.Id == emailId
                                                                            && q.SentAt != null
                                                                            && !q.IsClicked);
        
        if (email == null)
            return;
        
        if (DateTime.UtcNow - email.SentAt!.Value < TimeSpan.FromMinutes(2))
            return;

        await using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            email.IsClicked = true;
            email.ClickedAt = DateTime.UtcNow;

            _dbContext.AttackEmails.Update(email);

            var conversation = await _dbContext.Conversations
                             .SingleOrDefaultAsync(q => q.Id == email.ConversationId)
                         ?? throw BusinessException.NotFound(nameof(Conversation), email.ConversationId);

            conversation.IsClicked = true;

            _dbContext.Conversations.Update(conversation);

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

        var conversations = await _dbContext.Conversations
            .AsNoTracking()
            .GroupBy(q => q.AttackId)
            .Select(q => new
            {
                AttackId = q.Key,
                SuccessCount = q.Count(i => i.IsOpened || i.IsClicked || i.IsReplied),
                TargetCount = q.Count()
            })
            .ToArrayAsync();

        var response = (from attack in attacks
                join target in conversations on attack.Id equals target.AttackId
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
        
        var conversation = await _dbContext.Conversations
                         .SingleOrDefaultAsync(q => q.Id == email.ConversationId)
                     ?? throw BusinessException.NotFound(nameof(Conversation), email.ConversationId);

        await using var ts = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            email.IsReplied = true;
            email.RepliedAt = DateTime.UtcNow;

            _dbContext.AttackEmails.Update(email);
            
            conversation.IsReplied = true;

            _dbContext.Conversations.Update(conversation);

            var reply = new AttackEmailReply
            {
                Id = Guid.NewGuid(),
                ConversationId = conversation.Id,
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

        var conversation = await _dbContext.Conversations
                         .Select(c => new
                         {
                             c.Id,
                             c.ClientTargetId,
                             c.AttackId,
                             c.ClientTarget.Email,
                             c.ClientTarget.FullName
                         })
                         .SingleOrDefaultAsync(q => q.Id == email.ConversationId)
                     ?? throw BusinessException.NotFound(nameof(Conversation), email.ConversationId);

        var attack = await _dbContext.Attacks
                         .AsNoTracking()
                         .SingleOrDefaultAsync(q => q.Id == conversation.AttackId)
                     ?? throw BusinessException.NotFound(nameof(Attack), conversation.AttackId);
        
        var appUrl = _configuration.GetValue<string>("ApiBaseUrl")!;
        var newEmailId = Guid.NewGuid();
        var linkUrl = $"{appUrl}/api/webhooks/clicked/{newEmailId}";
        
        var response = await _phishingAiApiClient.GetReplyEmailContentAsync(attack.Language, new PhishingAiGetReplyEmailContentRequest(
            conversation.FullName,
            conversation.Email,
            newEmailId,
            linkUrl,
            reply.Subject,
            reply.Body,
            conversation.Id));

        var newEmail = new AttackEmail
        {
            Id = newEmailId,
            ConversationId = email.ConversationId,
            State = EmailStateEnum.Created,
            From = email.From,
            DisplayName = email.DisplayName,
            To = conversation.Email,
            Subject = reply.Subject,
            Body = response,
            CreatedAt = DateTime.UtcNow,
            SendAt = DateTime.UtcNow.AddMinutes(new Random().Next(_replyMinDuration, _replyMaxDuration)),
            AttackEmailReplyId = reply.Id,
            TryCount = 0
        };

        await _dbContext.AttackEmails.AddAsync(newEmail);

        await _dbContext.SaveChangesAsync();
    }
}