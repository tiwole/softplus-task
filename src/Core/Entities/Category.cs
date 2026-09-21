namespace Core.Entities;

public sealed class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
}
