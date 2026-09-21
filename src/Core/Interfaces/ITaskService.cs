using Shared.Tasks;

namespace Core.Interfaces;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetPagedAsync(TaskQuery query, CancellationToken cancellationToken);
}
