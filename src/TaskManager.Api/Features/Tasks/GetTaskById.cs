using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Tasks;

public static class GetTaskById
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

        public async Task<Response> ExecuteAsync(int id)
        {
            var task = await _db.Tasks
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                throw new KeyNotFoundException("Tarea no encontrada.");
            }

            return task.ToGetByIdResponse();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/tasks/{id:int}", async (int id, Handler handler) =>
        {
            try
            {
                var response = await handler.ExecuteAsync(id);
                return Results.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithName("GetTaskById")
        .WithTags("Tasks")
        .Produces<Response>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
