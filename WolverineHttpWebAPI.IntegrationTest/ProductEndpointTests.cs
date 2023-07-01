using Alba;
using WolverineHttpWebAPI.Entities;

namespace WebAPI.IntegrationTests;

[Collection(nameof(SharedCollectionFixture))]
public sealed class ProductEndpointTests : EndpointTestBase
{
    private readonly CreateProduct _createProduct;

    public ProductEndpointTests(AlbaHostFixture fixture) : base(fixture)
    {
        _createProduct = new CreateProduct
        {
            Name         = "ProductName",
            Price        = 50,
            Description  = "ProductDescription",
            CategoryEnum = CategoryEnum.Category1
        };
    }

    [Fact]
    public async Task Create_and_Get_Product()
    {
        // Act #1
        string location = await assumeProductCreated(_createProduct);

        // Assert
        Assert.NotEmpty(location);

        // Act #2
        ProductDto? productDto = await _albaHost.GetAsJson<ProductDto>(location);

        // Assert
        Assert.NotNull(productDto);
        Assert.Equal(_createProduct.Name, productDto.Name);
        // ...
    }

    private async Task<string> assumeProductCreated(CreateProduct createProduct)
    {
        IScenarioResult scenarioResult = await _albaHost.Scenario(scenario =>
        {
            scenario.Post.Json(createProduct).ToUrl("/api/Product");
            scenario.StatusCodeShouldBe(Status201Created);
        });

        return scenarioResult.Context.Response.Headers.Location.SingleOrDefault() ?? string.Empty;
    }
}