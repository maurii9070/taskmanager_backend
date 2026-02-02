using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;
using TaskManager.Api.Entities;

namespace TaskManager.Api.Features.Tasks;

public static class CreateTask
{
    public record Request(
        string Title,
        string Description,
        string Priority,
        DateTime DueDate,
        int CategoryId
    );

    public record Response(
        int Id,
        string Title
    );

    public class Handler
    {
        private readonly AppDbContext _db;

        public Handler(AppDbContext db) => _db = db;

        public async Task<Response> ExecuteAsync(Request request)
        {
            if (!Enum.TryParse<Common.TaskPriority>(request.Priority, true, out var priority))
            {
                priority = Common.TaskPriority.Medium;
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

            var taskItem = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                Priority = priority,
                DueDate = request.DueDate,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.UtcNow
            };

            _db.Tasks.Add(taskItem);
            await _db.SaveChangesAsync();

            return new Response(
                taskItem.Id,
                taskItem.Title
            );
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/tasks", async (Request request, Handler handler) =>
        {
            try
            {
                var response = await handler.ExecuteAsync(request);
                return Results.Created($"/api/tasks/{response.Id}", response);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.BadRequest(ex.Message);
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

            .WithName("CreateTask")
            .WithTags("Tasks")
            .Produces<Response>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
    }

}
