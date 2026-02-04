namespace TaskManager.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        Features.Tasks.CreateTask.MapEndpoint(app);
        Features.Tasks.GetAllTasks.MapEndpoint(app);
        Features.Tasks.GetTaskById.MapEndpoint(app);
        Features.Tasks.UpdateTask.MapEndpoint(app);
        Features.Tasks.UpdateTaskStatus.MapEndpoint(app);
        Features.Tasks.DeleteTask.MapEndpoint(app);
    }

    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        Features.Categories.CreateCategory.MapEndpoint(app);
        Features.Categories.GetAllCategories.MapEndpoint(app);
        Features.Categories.GetCategoryById.MapEndpoint(app);
    }
}
