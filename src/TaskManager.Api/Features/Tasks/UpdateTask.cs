using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Tasks;

public static class UpdateTask
{
    public record Request(
        string Title,
        string Description,
        string Status,
        string Priority,
        DateTime DueDate,
        int CategoryId
    );

    public record Response(
        int Id,
        string Title,
        string Description,
        string Status,
        string Priority,
        DateTime DueDate,
        int CategoryId,
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

            if (!Enum.TryParse<Common.TaskPriority>(request.Priority, true, out var priority))
            {
                throw new ArgumentException("Prioridad inválida.");
            }

            if (!Enum.TryParse<Common.TaskStatus>(request.Status, true, out var status))
            {
                throw new ArgumentException("Estado inválido.");
            }

            var categoryExists = await _db.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                throw new KeyNotFoundException("Categoria no encontrada.");
            }

            if (request.DueDate < DateTime.UtcNow)
            {
                throw new ArgumentException("La fecha de vencimiento no puede ser en el pasado.");
            }

            task.UpdateFromRequest(request, status, priority);

            await _db.SaveChangesAsync();

            return task.ToUpdateResponse();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/tasks/{id:int}", async (int id, Request request, Handler handler) =>
        {
            try
            {
                var response = await handler.ExecuteAsync(id, request);
                return Results.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithName("UpdateTask")
        .WithTags("Tasks")
        .Produces<Response>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
