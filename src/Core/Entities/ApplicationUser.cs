namespace Core.Entities;

public sealed class ApplicationUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();
}
