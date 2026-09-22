namespace Shared.Models;

public class PaginatedRequest
{
    public int Start { get; set; }

    public int Limit { get; set; } = 20;
}
