using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Wolverine.Tracking;

namespace WolverineHttpWithMarten.IntegrationTest.Core;

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

    protected async Task<(ITrackedSession, IScenarioResult)> trackedHttpCall(Action<Scenario> configuration)
    {
        IScenarioResult? scenarioResult = null;

        // ExecuteAndWait "wait" for all detected messages activity to complete
        ITrackedSession trackedSession = await _albaHost.ExecuteAndWaitAsync(async () =>
        {
            // Making a HTTP request with Alba
            scenarioResult = await _albaHost.Scenario(configuration);
        });

        if (scenarioResult is null)
        {
            throw new NullReferenceException(nameof(scenarioResult));
        }

        return (trackedSession, scenarioResult);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}
