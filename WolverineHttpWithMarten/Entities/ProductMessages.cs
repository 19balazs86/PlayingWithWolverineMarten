using System.Text.Json.Serialization;

namespace WolverineHttpWithMarten.Entities;

public class CreateProduct
{
    public string Name { get; init; } = string.Empty;
    public int Price { get; init; }
    public string Description { get; init; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CategoryEnum CategoryEnum { get; init; }
}

public interface IProductLookup
{
    int Id { get; init; }
}

// Validation automatically applied due to the inheritance
public sealed class UpdateProduct : CreateProduct, IProductLookup
{
    public int Id { get ; init ; }
}

public sealed record ProductCreated(int Id)
{
    public static ProductCreated FromId(int Id)
    {
        return new ProductCreated(Id);
    }
}

public sealed record ProductUpdated(int Id)
{
    public static ProductUpdated FromId(int Id)
    {
        return new ProductUpdated(Id);
    }
}