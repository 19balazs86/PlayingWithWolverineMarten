using WolverineHttpWithMarten.Entities;

namespace WolverineHttpWithMarten.Handlers;

public static class ProductEventHandler
{
    public static void Handle(ProductCreated productCreated, ILogger logger)
    {
        logger.LogInformation("Product is created with id: {id}", productCreated.Id);
    }

    public static void Handle(ProductUpdated productUpdated, ILogger logger)
    {
        logger.LogInformation("Product({id}) is updated", productUpdated.Id);
    }
}
