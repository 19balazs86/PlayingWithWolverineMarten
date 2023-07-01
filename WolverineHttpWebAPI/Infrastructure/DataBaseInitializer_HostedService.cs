using Bogus;
using Marten;
using WolverineHttpWebAPI.Entities;

namespace WolverineHttpWebAPI.Infrastructure;

public sealed class DataBaseInitializer_HostedService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DataBaseInitializer_HostedService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        IDocumentSession documentDb = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        bool hasAnyDocument = await documentDb.Query<Product>().AnyAsync();

        if (hasAnyDocument)
            return;

        await documentDb.DocumentStore.SeedProductAsync(100, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

public static class DocumentStoreExtensions
{
    public static async Task SeedProductAsync(this IDocumentStore documentStore, int count, CancellationToken cancellationToken)
    {
        var products = new Faker<Product>()
            //.RuleFor(p => p.Id, _ => id++) // No need. The auto increment works well.
            .RuleFor(p => p.Name,         f => f.Commerce.ProductName())
            .RuleFor(p => p.Price,        f => f.Random.Number(10, 500))
            .RuleFor(p => p.Description,  f => f.Lorem.Sentence())
            .RuleFor(p => p.CreatedDate,  f => f.Date.Recent(10).ToUniversalTime())
            .RuleFor(p => p.CategoryEnum, f => f.PickRandom<CategoryEnum>())
            .Generate(count);

        await documentStore.BulkInsertAsync(products, batchSize: products.Count, cancellation: cancellationToken);
    }
}
