using TaskManager.Api.Common;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Features.Tasks;

public static class TaskMappingExtensions
{
    public static TaskItem ToTaskItem(this CreateTask.Request request, TaskPriority priority)
    {
        return new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            Priority = priority,
            DueDate = request.DueDate,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static CreateTask.Response ToCreateResponse(this TaskItem task)
    {
        return new CreateTask.Response(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.DueDate,
            task.CategoryId,
            task.CreatedAt
        );
    }

    public static GetAllTasks.Response ToGetAllResponse(this TaskItem task)
    {
        return new GetAllTasks.Response(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.DueDate,
            task.CategoryId,
            task.Category?.Name ?? "Sin categoría",
            task.CreatedAt
        );
    }

    public static void UpdateFromRequest(
        this TaskItem task,
        UpdateTask.Request request,
        Common.TaskStatus status,
        TaskPriority priority)
    {
        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = status;
        task.Priority = priority;
        task.DueDate = request.DueDate;
        task.CategoryId = request.CategoryId;
    }

    public static UpdateTask.Response ToUpdateResponse(this TaskItem task)
    {
        return new UpdateTask.Response(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.DueDate,
            task.CategoryId,
            task.CreatedAt
        );
    }

    public static GetTaskById.Response ToGetByIdResponse(this TaskItem task)
    {
        return new GetTaskById.Response(
            task.Id,
            task.Title,
            task.Description,
            task.Status.ToString(),
            task.Priority.ToString(),
            task.DueDate,
            task.CategoryId,
            task.Category?.Name ?? "Sin categoría",
            task.CreatedAt
        );
    }

    public static UpdateTaskStatus.Response ToUpdateStatusResponse(this TaskItem task)
    {
        return new UpdateTaskStatus.Response(
            task.Id,
            task.Title,
            task.Status.ToString(),
            task.CreatedAt
        );
    }
}
