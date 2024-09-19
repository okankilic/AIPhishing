using AIPhishing.Business.Auth.Models;
using AIPhishing.Business.Configurations;
using AIPhishing.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AIPhishing.Business.Auth;

public class AuthBusiness : IAuthBusiness
{
    private readonly AdminConfiguration _adminConfiguration;
    private readonly IConfiguration _configuration;

    public AuthBusiness(
        IOptions<AdminConfiguration> adminConfigurationOptions,
        IConfiguration configuration)
    {
        _adminConfiguration = adminConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(adminConfigurationOptions));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (string.IsNullOrEmpty(request.UserName))
            throw BusinessException.Required(nameof(request.UserName));
        
        if (string.IsNullOrEmpty(request.Password))
            throw BusinessException.Required(nameof(request.Password));

        if (_adminConfiguration.UserName != request.UserName || _adminConfiguration.Password != request.Password)
            throw new UnauthorizedAccessException($"User name or password is invalid.");

        return Task.FromResult(new AuthLoginResponse(_configuration.GetValue<string>("ApiKey")));
    }
}