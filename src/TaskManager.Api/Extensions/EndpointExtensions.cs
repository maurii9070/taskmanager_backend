namespace TaskManager.Api.Extensions;

public static class EndpointExtensions
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        Features.Tasks.CreateTask.MapEndpoint(app);
        Features.Tasks.GetAllTasks.MapEndpoint(app);
    }
}
