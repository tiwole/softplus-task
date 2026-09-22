using Shared.Models;

namespace Shared.Tasks;

public class GetTasksRequest : PaginatedRequest
{
    public string? Search { get; set; }

    public Guid? CategoryId { get; set; }
}
