using WolverineHttpWebAPI.Entities;

namespace WolverineHttpWebAPI.Handlers;

public static class ProductCreatedHandler
{
    public static void Handle(ProductCreated productCreated, ILogger logger)
    {
        logger.LogInformation("Product is created with id: {id}", productCreated.Id);
    }
}
