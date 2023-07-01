using Marten;
using Marten.Schema;
using WolverineHttpWebAPI.Infrastructure;

namespace WolverineHttpWebAPI.IntegrationTest.Core;

public sealed class InitialProductData : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        await store.SeedProductAsync(5, cancellation);
    }
}
