using System.Security.Claims;
using AIPhishing.Business.Auth.Models;
using AIPhishing.Business.Configurations;
using AIPhishing.Business.Contexts;
using AIPhishing.Common.Constants;
using AIPhishing.Common.Exceptions;
using AIPhishing.Common.Helpers;
using AIPhishing.Database;
using AIPhishing.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AIPhishing.Business.Auth;

public class AuthBusiness : IAuthBusiness
{
    private readonly PhishingDbContext _dbContext;
    private readonly AdminConfiguration _adminConfiguration;
    private readonly JwtConfiguration _jwtConfiguration;

    public AuthBusiness(
        PhishingDbContext dbContext,
        IOptions<AdminConfiguration> adminConfigurationOptions,
        IOptions<JwtConfiguration> jwtConfigurationOptions)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _adminConfiguration = adminConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(adminConfigurationOptions));
        _jwtConfiguration = jwtConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(jwtConfigurationOptions));
    }

    public async Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        
        if (string.IsNullOrEmpty(request.UserName))
            throw BusinessException.Required(nameof(request.UserName));
        
        if (string.IsNullOrEmpty(request.Password))
            throw BusinessException.Required(nameof(request.Password));

        var user = await _dbContext.Users
                       .AsNoTracking()
                       .SingleOrDefaultAsync(q => q.Email == request.UserName)
                   ?? throw new UnauthorizedAccessException($"User name or password is invalid.");
        
        if (!PasswordHelper.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException($"User name or password is invalid.");
        
        List<Claim> claims =
        [
            new Claim(AuthClaimTypes.UserId, user.Id.ToString()),
            new Claim(AuthClaimTypes.Email, user.Email)
        ];
        
        if (user.ClientId != null)
            claims.Add(new Claim(AuthClaimTypes.ClientId, user.ClientId!.ToString() ?? string.Empty));

        var token = JwtTokenHelper.Generate(
            _jwtConfiguration.Secret,
            _jwtConfiguration.Issuer,
            _jwtConfiguration.Audience,
            _jwtConfiguration.ExpiresInMinutes,
            claims);

        return new AuthLoginResponse(
            token,
            _jwtConfiguration.ExpiresInMinutes,
            DateTime.UtcNow.AddMinutes(_jwtConfiguration.ExpiresInMinutes), 
            new AuthUserResponse(user.Email, user.ClientId));
    }

    public async Task UpdatePasswordAsync(AuthUpdatePasswordRequest request, UserContext currentUser)
    {
        if (request == null)
            throw BusinessException.Required(nameof(request));

        if (string.IsNullOrEmpty(request.OldPassword))
            throw BusinessException.Required(nameof(request.OldPassword));
        
        if (string.IsNullOrEmpty(request.NewPassword))
            throw BusinessException.Required(nameof(request.NewPassword));

        var user = await _dbContext.Users
                       .SingleOrDefaultAsync(q => q.Id == currentUser.Id)
                   ?? throw BusinessException.NotFound(nameof(User), currentUser.Id);
        
        if (PasswordHelper.Verify(request.OldPassword, user.Password))
            throw new BusinessException($"Your old password is invalid.");

        user.Password = PasswordHelper.Hash(request.NewPassword);

        _dbContext.Users.Update(user);

        await _dbContext.SaveChangesAsync();
    }
}