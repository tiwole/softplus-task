using Core.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Infrastructure;

public class UserRequestContext(IHttpContextAccessor httpContextAccessor) : IUserRequestContext
{
    public Guid UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim =>
                claim.Type == JwtRegisteredClaimNames.Sub || claim.Type == ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdClaim?.Value, out var userId))
            {
                return userId;
            }

            throw new NoUserIdInHttpContextException("Unauthorized");
        }
    }
}
