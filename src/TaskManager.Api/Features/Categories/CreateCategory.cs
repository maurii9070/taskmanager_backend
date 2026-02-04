using Microsoft.EntityFrameworkCore;

using TaskManager.Api.Database;

namespace TaskManager.Api.Features.Categories;

public static class CreateCategory
{
    public record Request(
        string Name
    );

    public record Response(
        int Id,
        string Name
    );

    public class Handler
    {
        private readonly AppDbContext _db;

        public Handler(AppDbContext db) => _db = db;

        public async Task<Response> ExecuteAsync(Request request)
        {
            var categoryExists = await _db.Categories
                .AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());

            if (categoryExists)
            {
                throw new ArgumentException("Ya existe una categoría con ese nombre.");
            }

            var category = request.ToCategoryItem();

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return category.ToCreateResponse();
        }
    }

    // Endpoint
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/categories", async (Request request, Handler handler) =>
        {
            try
            {
                var response = await handler.ExecuteAsync(request);
                return Results.Created($"/api/categories/{response.Id}", response);
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
        .WithName("CreateCategory")
        .WithTags("Categories")
        .Produces<Response>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
