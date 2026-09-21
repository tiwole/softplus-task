using Core.Interfaces;
using Shared.Tasks;

namespace Core.Services;

public sealed class TaskService : ITaskService
{
    public Task<PagedResult<TaskDto>> GetPagedAsync(TaskQuery query, CancellationToken cancellationToken)
    {
        var result = new PagedResult<TaskDto>(Array.Empty<TaskDto>(), query.PageNumber, query.PageSize, 0);
        return Task.FromResult(result);
    }
}
