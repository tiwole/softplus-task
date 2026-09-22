namespace Shared.Tasks;

public class TaskQuery
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public Guid? CategoryId { get; set; }
}
