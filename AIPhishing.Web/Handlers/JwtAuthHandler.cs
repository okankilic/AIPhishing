using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using AIPhishing.Business.Configurations;
using AIPhishing.Common.Constants;
using AIPhishing.Common.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;

namespace AIPhishing.Web.Handlers;

public class JwtAuthHandler : AuthenticationHandler<JwtAuthHandlerSchemeOptions>
{
    public const string AuthenticationScheme = "Phishing-Auth";
    public const string ApiKeyHeaderName = "X-API-Key";
    
    private readonly JwtConfiguration _jwtConfiguration;
    
    public JwtAuthHandler(
        IOptions<JwtConfiguration> jwtConfigurationOptions,
        IOptionsMonitor<JwtAuthHandlerSchemeOptions> options, 
        ILoggerFactory logger, 
        UrlEncoder encoder, 
        ISystemClock clock) 
        : base(options, logger, encoder, clock)
    {
        _jwtConfiguration = jwtConfigurationOptions?.Value ?? throw new ArgumentNullException(nameof(jwtConfigurationOptions));
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey(ApiKeyHeaderName))
            return AuthenticateResult.Fail("Authorization header not found.");

        try
        {
            var token = Request.Headers[ApiKeyHeaderName].ToString();

            var endpoint = Context.Features.Get<IEndpointFeature>()?.Endpoint;

            if (endpoint == null)
                return AuthenticateResult.Fail("Endpoint feature not found.");

            var authorizeAttribute = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();

            if (authorizeAttribute == null)
                return AuthenticateResult.Fail("Authorize attribute not found");

            var securityToken = JwtTokenHelper.Validate(
                _jwtConfiguration.Secret,
                _jwtConfiguration.Issuer,
                _jwtConfiguration.Audience,
                token);

            if (securityToken == null)
                return AuthenticateResult.Fail("Authorization failed.");
            
            var jwtToken = (JwtSecurityToken)securityToken;

            List<Claim> claims = [];
            
            var jwtTokenClaims = jwtToken.Claims
                .GroupBy(c => c.Type)
                .ToDictionary(c => c.Key, c => string.Join(',', c.Select(q => q.Value)));

            foreach (var claim in jwtTokenClaims)
            {
                var values = claim.Value.Split(',');

                foreach (var value in values)
                {
                    claims.Add(new Claim(claim.Key, value));
                }
            }

            ClaimsIdentity identity = new(claims, Scheme.Name, AuthClaimTypes.Name, null);

            ClaimsPrincipal principal = new(identity);

            AuthenticationTicket ticket = new(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex);
        }
    }
}

public class JwtAuthHandlerSchemeOptions : AuthenticationSchemeOptions
{
}