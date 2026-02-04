using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Categories;

public static class GetCategoryById
{
    public record Response(
        int Id,
        string Name,
        int TasksCount
    );

    public class Handler
    {
        private readonly AppDbContext _db;

        public Handler(AppDbContext db) => _db = db;

        public async Task<Response> ExecuteAsync(int id)
        {
            var category = await _db.Categories
                .Include(c => c.Tasks)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException("Categoría no encontrada.");
            }

            return category.ToGetByIdResponse();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories/{id:int}", async (int id, Handler handler) =>
        {
            try
            {
                var response = await handler.ExecuteAsync(id);
                return Results.Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.InternalServerError(ex.Message);
            }
        })
        .WithName("GetCategoryById")
        .WithTags("Categories")
        .Produces<Response>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
