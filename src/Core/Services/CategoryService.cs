using Core.Interfaces;
using Shared.Categories;

namespace Core.Services;

public class CategoryService : ICategoryService
{
    public Task<IReadOnlyCollection<CategoryDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<CategoryDto>>(Array.Empty<CategoryDto>());
    }
}
