namespace Shared.Tasks;

public sealed record TaskQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    Guid? CategoryId = null);
