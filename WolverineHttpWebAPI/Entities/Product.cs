namespace WolverineHttpWebAPI.Entities;

public enum CategoryEnum
{
    Category1 = 1, Category2, Category3
}

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedDate { get; set; }
    public CategoryEnum CategoryEnum { get; set; }
}
