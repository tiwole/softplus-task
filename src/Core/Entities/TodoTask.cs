namespace Core.Entities;

public sealed class TodoTask
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? DueDateUtc { get; set; }

    public Guid? CategoryId { get; set; }

    public Category? Category { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
