using Core.Entities;

namespace Core.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Category?> GetByNameAsync(string name, Guid? userId, CancellationToken cancellationToken);

    Task<Category> CreateAsync(Guid userId, string name, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task DeleteAsync(Category category, CancellationToken cancellationToken);
}
