using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
{
    public async Task<List<Category>> GetAllAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .Where(category => category.UserId == userId)
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken);
    }

    public Task<Category?> GetByNameAsync(string name, Guid? userId, CancellationToken cancellationToken)
    {
        return dbContext.Categories
            .Where(category => category.Name == name)
            .Where(category => category.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Category> CreateAsync(Guid userId, string name, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            UserId = userId
        };

        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return category;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Category category, CancellationToken cancellationToken)
    {
        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
