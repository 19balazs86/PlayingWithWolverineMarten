namespace WolverineHttpWithMarten.Entities;

public sealed class ProductDto
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public int Price { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedDate { get; set; }
    public string? CategoryEnum { get; init; }
}
