namespace Shared.Tasks;

public class TaskDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsCompleted { get; set; }

    public Guid? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? DueDateUtc { get; set; }
}
