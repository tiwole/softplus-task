using Shared.Categories;

namespace Core.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryModel>> GetAllAsync(CancellationToken cancellationToken);

    Task<CategoryModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<CategoryModel> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);

    Task<CategoryModel> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
