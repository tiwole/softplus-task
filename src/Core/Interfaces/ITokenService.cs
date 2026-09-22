using Core.Entities;

namespace Core.Interfaces;

public interface ITokenService
{
    (string Token, DateTime ExpiresAtUtc) Create(ApplicationUser user);
}
