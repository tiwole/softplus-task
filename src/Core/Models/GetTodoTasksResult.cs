using Core.Entities;

namespace Core.Models;

public class GetTodoTasksResult
{
    public List<TodoTask> Data { get; set; } = [];

    public int TotalCount { get; set; }

    public bool HasNextPage { get; set; }
}
