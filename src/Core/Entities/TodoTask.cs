namespace Core.Entities;

public class TodoTask
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? DueDateUtc { get; set; }

    
    public Guid UserId { get; set; }
    
    public Guid? CategoryId { get; set; }
}
