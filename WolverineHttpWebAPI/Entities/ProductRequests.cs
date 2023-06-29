namespace WolverineHttpWebAPI.Entities;

/// <summary>
/// Request for create a product.
/// </summary>
public sealed class CreateProduct
{
    public string Name { get; init; } = string.Empty;
    public int Price { get; init; }
    public string Description { get; init; } = string.Empty;
    public CategoryEnum CategoryEnum { get; init; }
}

public interface IProductLookup
{
    int Id { get; init; }
}

public readonly record struct DeleteProduct(int Id) : IProductLookup;