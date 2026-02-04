using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Categories;

public static class GetAllCategories
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

        public async Task<List<Response>> ExecuteAsync()
        {
            var categories = await _db.Categories
                .Include(c => c.Tasks)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return categories.Select(c => c.ToGetAllResponse()).ToList();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/categories", async (Handler handler) =>
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
        .WithName("GetAllCategories")
        .WithTags("Categories")
        .Produces<List<Response>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
