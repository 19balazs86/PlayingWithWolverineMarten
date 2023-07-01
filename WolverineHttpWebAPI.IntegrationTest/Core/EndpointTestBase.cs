using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;

namespace WolverineHttpWebAPI.IntegrationTest.Core;

public abstract class EndpointTestBase : IAsyncLifetime
{
    protected readonly AlbaHostFixture _fixture;

    protected readonly IAlbaHost _albaHost;

    protected readonly IDocumentStore _documentStore;

    public EndpointTestBase(AlbaHostFixture fixture)
    {
        _fixture  = fixture;
        _albaHost = fixture.AlbaWebHost;

        _documentStore = _albaHost.Services.GetRequiredService<IDocumentStore>();
    }

    public async Task InitializeAsync()
    {
        // Using Marten to (like with Respawn library in PlayingWithTestContainers)
        // 1) Wipe out all data
        // 2) Reset the state back when we use InitializeMartenWith<InitialProductData>()
        await _documentStore.Advanced.ResetAllData();
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
