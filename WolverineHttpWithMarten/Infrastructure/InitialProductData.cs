using Bogus;
using Marten;
using Marten.Schema;
using WolverineHttpWithMarten.Entities;

namespace WolverineHttpWithMarten.Infrastructure;

public sealed class InitialProductData : IInitialData
{
    public const int ProductCount_Dev             = 100;
    public const int ProductCount_IntegrationTest = 5;

    private readonly int _productCount;

    public InitialProductData(int productCount)
    {
        _productCount = productCount;
    }

    public static InitialProductData Create(int productCount = ProductCount_Dev)
    {
        return new InitialProductData(productCount);
    }

    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        using IQuerySession querySession = store.QuerySession();

        bool hasAnyDocument = await querySession.Query<Product>().AnyAsync(cancellation);

        if (hasAnyDocument)
            return;

        await seedProductAsync(store, _productCount, cancellation);
    }

    private async Task seedProductAsync(IDocumentStore documentStore, int count, CancellationToken cancellationToken)
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

public static class InfrastructureExtensions
{
    public static Target ApplyWhen<Target>(this Target target, bool canApply, Action<Target> applyAction)
    {
        if (canApply)
        {
            applyAction(target);
        }

        return target;
    }
}
