using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Shared.Categories;

namespace Core.Services;

public class CategoryService(ICategoryRepository categoryRepository, IUserRequestContext userContext) : ICategoryService
{
    public async Task<IReadOnlyCollection<CategoryModel>> GetAllAsync(CancellationToken cancellationToken)
    {
        // maybe useless
        var categories = await categoryRepository.GetAllAsync(userContext.UserId, cancellationToken);

        return categories
            .Select(category => new CategoryModel
            {
                Id = category.Id,
                Name = category.Name
            })
            .ToArray();
    }

    public async Task<CategoryModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await GetCategoryOrThrowAsync(id, cancellationToken);
        if (category.UserId != userContext.UserId)
            throw new CategoryException("Forbidden", 403);

        return new CategoryModel
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryModel> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        await ThrowIfNameExistsAsync(request.Name, cancellationToken);

        var category = await categoryRepository.CreateAsync(userContext.UserId, request.Name, cancellationToken);

        return new CategoryModel
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task<CategoryModel> UpdateAsync(Guid id, UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await GetCategoryOrThrowAsync(id, cancellationToken);
        await ThrowIfNameExistsAsync(request.Name, cancellationToken);

        category.Name = request.Name;
        await categoryRepository.SaveChangesAsync(cancellationToken);

        return new CategoryModel
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await GetCategoryOrThrowAsync(id, cancellationToken);

        if (category.UserId != userContext.UserId)
            throw new CategoryException("Forbidden", 403);
        
        await categoryRepository.DeleteAsync(category, cancellationToken);
    }
    
    private async Task<Category> GetCategoryOrThrowAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is null)
            throw new CategoryException("Category not found", 404);

        return category;
    }

    private async Task ThrowIfNameExistsAsync(string name, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByNameAsync(name, userContext.UserId, cancellationToken);
        if (category is not null)
            throw new CategoryException("Category with that name already exists", 409);
    }
}