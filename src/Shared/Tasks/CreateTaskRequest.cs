namespace Shared.Tasks;

public class CreateTaskRequest
{
    public string Title { get; set; } = string.Empty;

    public DateTime? DueDateUtc { get; set; }

    public Guid? CategoryId { get; set; }
}
