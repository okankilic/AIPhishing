using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AIPhishing.Common.Helpers;

public static class JwtTokenHelper
{
    public static string Generate(
        string secret,
        string issuer,
        string audience, 
        int expiresInMinutes,
        IList<Claim> claims)
    {
        if(string.IsNullOrEmpty(secret))
            throw new ArgumentNullException(nameof(secret));

        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentNullException(nameof(issuer));

        if (string.IsNullOrEmpty(audience))
            throw new ArgumentNullException(nameof(audience));

        claims ??= new List<Claim>();

        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));

        var key = Encoding.ASCII.GetBytes(secret);

        SymmetricSecurityKey symmetricSecurityKey = new(key);

        SigningCredentials signingCredentials = new(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expiresInMinutes),
            NotBefore = DateTime.UtcNow,
            SigningCredentials = signingCredentials,
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public static SecurityToken? Validate(
        string secret,
        string issuer,
        string audience,
        string? token)
    {
        if (string.IsNullOrEmpty(secret))
            throw new ArgumentNullException(nameof(secret));

        if (string.IsNullOrEmpty(issuer))
            throw new ArgumentNullException(nameof(issuer));

        if (string.IsNullOrEmpty(audience))
            throw new ArgumentNullException(nameof(audience));

        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(secret);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true
            }, out SecurityToken validatedToken);

            return validatedToken;
        }
        catch
        {
            return null;
        }
    }
}