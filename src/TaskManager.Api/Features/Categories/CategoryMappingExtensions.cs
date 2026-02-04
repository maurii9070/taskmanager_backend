using TaskManager.Api.Entities;

namespace TaskManager.Api.Features.Categories;

public static class CategoryMappingExtensions
{
    public static Category ToCategoryItem(this CreateCategory.Request request)
    {
        return new Category
        {
            Name = request.Name
        };
    }

    public static CreateCategory.Response ToCreateResponse(this Category category)
    {
        return new CreateCategory.Response(
            category.Id,
            category.Name
        );
    }

    public static GetAllCategories.Response ToGetAllResponse(this Category category)
    {
        return new GetAllCategories.Response(
            category.Id,
            category.Name,
            category.Tasks.Count
        );
    }

    public static GetCategoryById.Response ToGetByIdResponse(this Category category)
    {
        return new GetCategoryById.Response(
            category.Id,
            category.Name,
            category.Tasks.Count
        );
    }
}
