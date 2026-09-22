namespace Shared.Models;

public class PaginatedResponse<T>
{
    public IReadOnlyCollection<T> Data { get; set; } = Array.Empty<T>();

    public int TotalCount { get; set; }

    public bool HasNextPage { get; set; }
}
