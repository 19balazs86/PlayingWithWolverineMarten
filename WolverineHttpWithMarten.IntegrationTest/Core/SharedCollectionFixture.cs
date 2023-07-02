namespace WolverineHttpWithMarten.IntegrationTest.Core
{
    [CollectionDefinition(nameof(SharedCollectionFixture))]
    public class SharedCollectionFixture : ICollectionFixture<AlbaHostFixture>
    {
        // I use CollectionFixture insetad of IClassFixture.
        // For multiple test classes the "host" needs to be shared, not just for 1 class with IClassFixture.
    }
}
