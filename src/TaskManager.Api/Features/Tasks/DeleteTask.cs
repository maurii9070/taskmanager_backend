using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Tasks;

public static class DeleteTask
{
    public class Handler
    {
        private readonly AppDbContext _db;

        public Handler(AppDbContext db) => _db = db;

        public async Task ExecuteAsync(int id)
        {
            var task = await _db.Tasks.FindAsync(id);

            if (task == null)
            {
                throw new KeyNotFoundException("Tarea no encontrada.");
            }

            _db.Tasks.Remove(task);
            await _db.SaveChangesAsync();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/tasks/{id:int}", async (int id, Handler handler) =>
        {
            try
            {
                await handler.ExecuteAsync(id);
                return Results.NoContent();
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
        .WithName("DeleteTask")
        .WithTags("Tasks")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
