namespace Core.Entities;

public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public List<TodoTask> Tasks { get; set; } = [];
}
