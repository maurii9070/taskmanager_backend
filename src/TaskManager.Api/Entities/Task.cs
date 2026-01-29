namespace TaskManager.Api.Entities;

using TaskManager.Api.Common;

public class Task
{
    public int? Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }

    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }

    public int CategoryId { get; set; }
    public required Category Category { get; set; }
}
