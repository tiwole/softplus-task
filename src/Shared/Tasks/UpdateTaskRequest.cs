namespace Shared.Tasks;

public class UpdateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? DueDateUtc { get; set; }

    public Guid? CategoryId { get; set; }
}
