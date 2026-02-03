using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Tasks;

public static class GetAllTasks
{
    public record Response(
        int Id,
        string Title,
        string Description,
        string Status,
        string Priority,
        DateTime DueDate,
        int CategoryId,
        string CategoryName,
        DateTime CreatedAt
    );

    public class Handler
    {
        private readonly AppDbContext _db;

        public Handler(AppDbContext db) => _db = db;

        public async Task<List<Response>> ExecuteAsync()
        {
            var tasks = await _db.Tasks
                .Include(t => t.Category)
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return tasks.Select(t => t.ToGetAllResponse()).ToList();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks", async (Handler handler) =>
        {
            try
            {
                var response = await handler.ExecuteAsync();
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithName("GetAllTasks")
        .WithTags("Tasks")
        .Produces<List<Response>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
