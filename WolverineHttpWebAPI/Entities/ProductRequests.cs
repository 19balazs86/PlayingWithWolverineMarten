namespace WolverineHttpWebAPI.Entities;

/// <summary>
/// Request for create a product.
/// </summary>
public class CreateProduct
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

// Validation automatically applied due to the inheritance
public sealed class UpdateProduct : CreateProduct, IProductLookup
{
    public int Id { get ; init ; }
}