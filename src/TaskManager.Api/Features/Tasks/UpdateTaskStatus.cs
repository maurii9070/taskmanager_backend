using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Tasks;

public static class UpdateTaskStatus
{
    public record Request(
        string Status
    );

    public record Response(
        int Id,
        string Title,
        string Status,
        DateTime CreatedAt
    );

    public class Handler
    {
        private readonly AppDbContext _db;

        public Handler(AppDbContext db) => _db = db;

        public async Task<Response> ExecuteAsync(int id, Request request)
        {
            var task = await _db.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new KeyNotFoundException("Tarea no encontrada.");
            }

            if (!Enum.TryParse<Common.TaskStatus>(request.Status, true, out var status))
            {
                throw new ArgumentException("Estado inválido. Valores permitidos: Pending, InProgress, Completed.");
            }

            task.Status = status;

            await _db.SaveChangesAsync();

            return task.ToUpdateStatusResponse();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/tasks/{id:int}/status", async (int id, Request request, Handler handler) =>
        {
            try
            {
                var response = await handler.ExecuteAsync(id, request);
                return Results.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithName("UpdateTaskStatus")
        .WithTags("Tasks")
        .Produces<Response>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
