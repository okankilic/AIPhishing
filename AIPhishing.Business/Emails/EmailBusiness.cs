using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using AIPhishing.Business.Configurations;
using AIPhishing.Common.Enums;
using AIPhishing.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AIPhishing.Business.Emails;

public class EmailBusiness : IEmailBusiness
{
    private readonly ILogger<IEmailBusiness> _logger;
    private readonly EmailConfiguration _emailConfiguration;
    private readonly PhishingDbContext _dbContext;

    public EmailBusiness( 
        ILogger<IEmailBusiness> logger,
        IOptions<EmailConfiguration> emailConfigurationOptions,
        PhishingDbContext dbContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _emailConfiguration = emailConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(emailConfigurationOptions));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SendManyAsync(CancellationToken cancellationToken = default)
    {
        EmailStateEnum[] states = [EmailStateEnum.Created, EmailStateEnum.Error]; 
        
        var emails = await _dbContext.AttackEmails
            .Where(q => q.SentAt == null
                        && states.Contains(q.State)
                        && q.TryCount < _emailConfiguration.MaxTryCount
                        && (q.SendAt == null || q.SendAt <= DateTime.UtcNow))
            .OrderBy(q => q.CreatedAt)
            .Take(_emailConfiguration.MaxFetchCount)
            .ToArrayAsync(cancellationToken: cancellationToken);

        foreach (var email in emails)
        {
            email.State = EmailStateEnum.Processing;
            email.TryCount++;
            
            try
            {
                await SendAsync(
                    email.Id,
                    email.From, 
                    email.DisplayName, 
                    email.To, 
                    email.Subject, 
                    email.Body);

                email.State = EmailStateEnum.Sent;
                email.SentAt = DateTime.UtcNow;
            }
            catch (Exception e)
            {
                email.State = EmailStateEnum.Error;
                email.ErrorMessage = e.Message;
            }
            finally
            {
                _dbContext.AttackEmails.Update(email);

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task SendAsync(Guid emailId, string from, string displayName, string to, string subject, string body)
    {
        try
        {
            var mail = new MailMessage
            {
                Sender = new MailAddress(_emailConfiguration.From),
                From = new MailAddress(from, displayName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            
            mail.Headers.Add("X-AiPhishing-EmailId", emailId.ToString());
            mail.To.Add(new MailAddress(to));
            
            using var smtpClient = new SmtpClient(_emailConfiguration.Host, _emailConfiguration.Port);
            
            smtpClient.Credentials = new NetworkCredential(_emailConfiguration.Username, _emailConfiguration.Password);
            smtpClient.EnableSsl = false;

            await smtpClient.SendMailAsync(mail);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"An error occured while sending the email.");
            throw;
        }
    }
}