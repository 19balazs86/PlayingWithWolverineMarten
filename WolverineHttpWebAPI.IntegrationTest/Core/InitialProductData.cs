using Marten;
using Marten.Schema;
using WolverineHttpWebAPI.Infrastructure;

namespace WolverineHttpWebAPI.IntegrationTest.Core;

public sealed class InitialProductData : IInitialData
{
    public const int InitialProductCount = 5;

    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await store.SeedProductAsync(InitialProductCount, cancellation);
    }
}
