using AIPhishing.Business.Emails;

namespace AIPhishing.Web.BackgroundServices;

public class EmailService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public EmailService(
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<EmailService>>();

            logger.LogInformation($"{nameof(EmailService)} started.");

            var emailBusiness = scope.ServiceProvider.GetRequiredService<IEmailBusiness>();

            await emailBusiness.SendManyAsync(cancellationToken);
            
            logger.LogInformation($"{nameof(EmailService)} ended.");

            await Task.Delay(10 * 1000, cancellationToken);
        }
    }
}