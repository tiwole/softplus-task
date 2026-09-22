using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);

    Task<ApplicationUser> CreateAsync(string email, string passwordHash, CancellationToken cancellationToken);
}
