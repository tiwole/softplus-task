using Core.Entities;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Infrastructure.Repositories;

public class TodoTaskRepository(ApplicationDbContext dbContext) : ITodoTaskRepository
{
    public async Task<GetTodoTasksResult> GetPagedAsync(PaginatedRequest request, Guid userId, CancellationToken cancellationToken)
    {
        var query = dbContext.Tasks
            .AsNoTracking()
            .Include(task => task.Category)
            .Where(task => task.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var tasks = await query
            .OrderBy(task => task.IsCompleted)
            .ThenBy(task => task.DueDateUtc ?? DateTime.MaxValue)
            .ThenByDescending(task => task.CreatedAtUtc)
            .Skip(request.Start)
            .Take(request.Limit + 1)
            .ToListAsync(cancellationToken);

        var hasNextPage = tasks.Count > request.Limit;
        if (hasNextPage)
            tasks.RemoveAt(tasks.Count - 1);

        return new GetTodoTasksResult
        {
            Data = tasks,
            TotalCount = totalCount,
            HasNextPage = hasNextPage
        };
    }

    public Task<TodoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.Tasks
            .Include(task => task.Category)
            .FirstOrDefaultAsync(task => task.Id == id, cancellationToken);
    }
    
    public Task<bool> IsTaskExistsByNameAsync(string title, Guid userId, CancellationToken cancellationToken)
    {
        return dbContext.Tasks.AnyAsync(task => task.Title == title && task.UserId == userId, cancellationToken);
    }

    public async Task<TodoTask> CreateAsync(Guid userId, string title, DateTime? dueDateUtc, Guid? categoryId, CancellationToken cancellationToken)
    {
        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            Title = title,
            DueDateUtc = dueDateUtc,
            CategoryId = categoryId,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return task;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TodoTask task, CancellationToken cancellationToken)
    {
        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
