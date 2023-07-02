using Marten.Metadata;

namespace WolverineHttpWebAPI.Entities;

public enum CategoryEnum
{
    Category1 = 1, Category2, Category3
}

// Soft delete -> https://martendb.io/documents/deletes.html#soft-deletes
public sealed class Product : ISoftDeleted
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public CategoryEnum CategoryEnum { get; set; }

    public bool Deleted { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
