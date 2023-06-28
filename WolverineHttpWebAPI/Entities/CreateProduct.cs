namespace WolverineHttpWebAPI.Entities;

/// <summary>
/// Request for create a product.
/// </summary>
public sealed class CreateProduct
{
    public string Name { get; init; } = string.Empty;
    public int Price { get; init; }
    public string Description { get; init; } = string.Empty;
}
