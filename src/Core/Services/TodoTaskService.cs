using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Shared.Models;
using Shared.Tasks;

namespace Core.Services;

public class TodoTaskService(
    ITodoTaskRepository taskRepository,
    ICategoryRepository categoryRepository,
    IUserRequestContext userContext) : ITodoTaskService
{
    public async Task<PaginatedResponse<TaskModel>> GetPagedAsync(PaginatedRequest request, CancellationToken cancellationToken)
    {
        var dataResult = await taskRepository.GetPagedAsync(request, userContext.UserId, cancellationToken);
        var data = dataResult.Data.Select(Map).ToArray();
        
        return new PaginatedResponse<TaskModel>
        {
            TotalCount = dataResult.TotalCount,
            Data = data,
            HasNextPage = dataResult.HasNextPage
        };
    }

    public async Task<TaskModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await GetTaskOrThrowAsync(id, cancellationToken);
        ThrowIfForbidden(task);

        return Map(task);
    }

    public async Task<TaskModel> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new TaskException("Task title is required", 400);
        await ThrowIfCategoryForbiddenOrMissingAsync(request.CategoryId, cancellationToken);
        
        var exists = await taskRepository.IsTaskExistsByNameAsync(request.Title, userContext.UserId, cancellationToken);
        if (exists)
            throw new TaskException("Task already exists", 400);

        var task = await taskRepository.CreateAsync(
            userContext.UserId,
            request.Title,
            request.DueDateUtc,
            request.CategoryId,
            cancellationToken);

        return Map(task);
    }

    public async Task<TaskModel> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            throw new TaskException("Task title is required", 400);
        await ThrowIfCategoryForbiddenOrMissingAsync(request.CategoryId, cancellationToken);

        var task = await GetTaskOrThrowAsync(id, cancellationToken);
        ThrowIfForbidden(task);

        task.Title = request.Title;
        task.IsCompleted = request.IsCompleted;
        task.DueDateUtc = request.DueDateUtc;
        task.CategoryId = request.CategoryId;

        await taskRepository.SaveChangesAsync(cancellationToken);

        return Map(task);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await GetTaskOrThrowAsync(id, cancellationToken);
        ThrowIfForbidden(task);

        await taskRepository.DeleteAsync(task, cancellationToken);
    }

    private async Task<TodoTask> GetTaskOrThrowAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
            throw new TaskException("Task not found", 404);

        return task;
    }

    private async Task ThrowIfCategoryForbiddenOrMissingAsync(Guid? categoryId, CancellationToken cancellationToken)
    {
        if (categoryId is null)
            return;

        var category = await categoryRepository.GetByIdAsync(categoryId.Value, cancellationToken);
        if (category is null)
            throw new TaskException("Category not found", 404);

        if (category.UserId != userContext.UserId)
            throw new TaskException("Forbidden", 403);
    }

    private void ThrowIfForbidden(TodoTask task)
    {
        if (task.UserId != userContext.UserId)
            throw new TaskException("Forbidden", 403);
    }

    private static TaskModel Map(TodoTask task)
    {
        return new TaskModel
        {
            Id = task.Id,
            Title = task.Title,
            IsCompleted = task.IsCompleted,
            CreatedAtUtc = task.CreatedAtUtc,
            DueDateUtc = task.DueDateUtc,
            CategoryId = task.CategoryId,
            CategoryName = task.Category?.Name
        };
    }
}
