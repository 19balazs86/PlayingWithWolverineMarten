using System.Linq.Expressions;

namespace WolverineHttpWebAPI.Entities;

public static class Mappers
{
    public static Product ToProduct(this CreateProduct cp)
    {
        return new Product
        {
            Name         = cp.Name,
            Price        = cp.Price,
            Description  = cp.Description,
            CreatedDate  = DateTime.UtcNow,
            CategoryEnum = CategoryEnum.Category1
        };
    }

    // It can be replaced with Mapster
    public static readonly Expression<Func<Product, ProductDto>> ProductToDtoProjection = p =>
        new ProductDto
        {
            Id           = p.Id,
            Name         = p.Name,
            Price        = p.Price,
            Description  = p.Description,
            CreatedDate  = p.CreatedDate,
            CategoryEnum = p.CategoryEnum.ToString()
        };
}
