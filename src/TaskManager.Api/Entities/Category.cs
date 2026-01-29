namespace TaskManager.Api.Entities;

public class Category
{
    public int Id { get; private set; }
    public required string Name { get; set; }

    public ICollection<TaskItem> Tasks { get; } = [];
}
