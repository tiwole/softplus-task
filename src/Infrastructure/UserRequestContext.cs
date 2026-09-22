using Core.Interfaces;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Infrastructure;

public class UserRequestContext(IHttpContextAccessor httpContextAccessor) : IUserRequestContext
{
    public Guid UserId
    {
        get
        {
            var userIdClaim = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == "sub");
            if (Guid.TryParse(userIdClaim?.Value, out var userId))
            {
                return userId;
            }

            throw new NoUserIdInHttpContextException("Unauthorized");
        }
    }
}
