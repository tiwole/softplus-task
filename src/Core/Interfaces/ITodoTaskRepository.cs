using Core.Entities;
using Core.Models;
using Shared.Tasks;

namespace Core.Interfaces;

public interface ITodoTaskRepository
{
    Task<GetTodoTasksResult> GetPagedAsync(GetTasksRequest request, Guid userId, CancellationToken cancellationToken);

    Task<TodoTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> IsTaskExistsByNameAsync(string title, Guid userId, CancellationToken cancellationToken);

    Task<TodoTask> CreateAsync(Guid userId, string title, DateTime? dueDateUtc, Guid? categoryId, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task DeleteAsync(TodoTask task, CancellationToken cancellationToken);
}
