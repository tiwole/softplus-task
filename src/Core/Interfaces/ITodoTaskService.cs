using Shared.Models;
using Shared.Tasks;

namespace Core.Interfaces;

public interface ITodoTaskService
{
    Task<PaginatedResponse<TaskModel>> GetPagedAsync(GetTasksRequest request, CancellationToken cancellationToken);

    Task<TaskModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<TaskModel> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken);

    Task<TaskModel> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
