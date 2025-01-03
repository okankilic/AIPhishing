﻿using AIPhishing.Business.Attacks;
using AIPhishing.Business.Attacks.Models;
using AIPhishing.Business.Integrations;
using AIPhishing.Business.Integrations.Models;
using AIPhishing.Common.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stateless;

namespace AIPhishing.Business.Managers;

public class AttackManager
{
    private enum Triggers
    {
        FetchEmailContents,
        CreateEmails,
        Complete
    }

    private enum States
    {
        Initial,
        EmailContentFetch,
        EmailCreate,
        Completed
    }

    private readonly States _initState = States.Initial;

    private readonly StateMachine<States, Triggers> _stateMachine;
    
    private readonly List<AttackEmailCreateModel> _emailModels = [];
    
    public AttackManager(IServiceProvider serviceProvider, Guid attackId)
    {
        var attackBusiness = serviceProvider.GetRequiredService<IAttackBusiness>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetRequiredService<ILogger<AttackManager>>();
        
        _stateMachine = new StateMachine<States, Triggers>(_initState);

        _stateMachine.Configure(States.Initial)
            .OnEntryAsync(async () =>
            {
                await _stateMachine.FireAsync(Triggers.FetchEmailContents);
            })
            .Permit(Triggers.FetchEmailContents, States.EmailContentFetch);
        
        _stateMachine.Configure(States.EmailContentFetch)
            .OnEntryAsync(async () =>
            {
                var attack = await attackBusiness.GetAsync(attackId);

                if (attack.State == AttackStateEnum.MailsCreated)
                {
                    await _stateMachine.FireAsync(Triggers.Complete);
                    return;
                }

                try
                {
                    _emailModels.Clear();
                
                    await attackBusiness.UpdateStateAsync(attackId, AttackStateEnum.FetchingMailContent);
                
                    var phishingAiApiClient = serviceProvider.GetRequiredService<IPhishingAiApiClient>();
                
                    var appUrl = configuration.GetValue<string>("ApiBaseUrl")!;
                    
                    foreach (var conversation in attack.Conversations)
                    {
                        if (string.IsNullOrEmpty(conversation.AttackType))
                            continue;
                 
                        var emailId = Guid.NewGuid();
                        var linkUrl = $"{appUrl}/api/webhooks/clicked/{emailId}";

                        var request = new PhishingAiGetEmailContentRequest(
                            conversation.AttackType, 
                            conversation.FullName,
                            conversation.Email, 
                            emailId, 
                            linkUrl,
                            conversation.Id);
                    
                        var response = await phishingAiApiClient.CreateEmailContentAsync(attack.Language, request);
                    
                        _emailModels.Add(new AttackEmailCreateModel(
                            conversation.Id,
                            emailId, 
                            conversation.Email, 
                            response.Sender, 
                            response.Sender, 
                            response.Subject, 
                            response.Content,
                            attack.StartTime));
                    }
                    
                    await attackBusiness.UpdateStateAsync(attackId, AttackStateEnum.MailContentFetched);
                    
                    await _stateMachine.FireAsync(Triggers.CreateEmails);
                }
                catch (Exception e)
                {
                    logger.LogError(e, $"An error occured while integrating with {nameof(PhishingAiApiClient)}");
                    
                    await attackBusiness.UpdateStateAsync(attackId, AttackStateEnum.Failed, e.Message);
                    
                    await _stateMachine.FireAsync(Triggers.Complete);
                }
            })
            .Permit(Triggers.CreateEmails, States.EmailCreate)
            .Permit(Triggers.Complete, States.Completed);

        _stateMachine.Configure(States.EmailCreate)
            .OnEntryAsync(async () =>
            {
                var attack = await attackBusiness.GetAsync(attackId);

                if (attack.State != AttackStateEnum.MailsCreated)
                {
                    await attackBusiness.UpdateStateAsync(attackId, AttackStateEnum.CreatingMails);
                
                    await attackBusiness.CreateEmailsAsync(attackId, _emailModels.ToArray());
                }
                
                await _stateMachine.FireAsync(Triggers.Complete);
            })
            .Permit(Triggers.Complete, States.Completed);

        _stateMachine.Configure(States.Completed)
            .OnEntryAsync(async () =>
            {
                await attackBusiness.UpdateStateAsync(attackId, AttackStateEnum.MailsCreated);
                
                var attack = await attackBusiness.GetAsync(attackId);

                logger.LogInformation($"Attack: {attackId} completed with state: {attack.State}");
            });
    }

    public Task FetchEmailContents()
    {
        return _stateMachine.FireAsync(Triggers.FetchEmailContents);
    }
}