namespace Shared.Tasks;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    Guid? CategoryId,
    string? CategoryName,
    DateTime CreatedAtUtc,
    DateTime? DueDateUtc);
