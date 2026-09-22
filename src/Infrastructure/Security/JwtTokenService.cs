using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Models;

namespace Infrastructure.Security;

public class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    public (string Token, DateTime ExpiresAtUtc) Create(ApplicationUser user)
    {
        var jwtOptions = options.Value;
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(jwtOptions.ExpiresMinutes);
        var credentials = new SigningCredentials(
            CreateSecurityKey(jwtOptions.Secret),
            SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };
        var token = new JwtSecurityToken(
            jwtOptions.Issuer,
            jwtOptions.Audience,
            claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAtUtc);
    }

    public ClaimsPrincipal? Validate(string token)
    {
        var jwtOptions = options.Value;
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = CreateSecurityKey(jwtOptions.Secret),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        try
        {
            return new JwtSecurityTokenHandler { MapInboundClaims = false }.ValidateToken(
                token,
                validationParameters,
                out _);
        }
        catch (SecurityTokenException)
        {
            return null;
        }
        catch (ArgumentException)
        {
            return null;
        }
    }

    private static SymmetricSecurityKey CreateSecurityKey(string secret)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    }
}
