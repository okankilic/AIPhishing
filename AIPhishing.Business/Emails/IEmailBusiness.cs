namespace AIPhishing.Business.Emails;

public interface IEmailBusiness
{
    Task SendManyAsync(CancellationToken cancellationToken = default);
}