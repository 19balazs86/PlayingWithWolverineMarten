namespace WolverineHttpWebAPI.Entities;

public record ProductCreated(int Id)
{
    public static ProductCreated FromId(int Id)
    {
        return new ProductCreated(Id);
    }
}
