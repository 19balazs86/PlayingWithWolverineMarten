using Alba;
using Marten;
using Wolverine.Tracking;
using WolverineHttpWithMarten.Entities;
using WolverineHttpWithMarten.Pagination;

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

    /// <summary>
    /// Test for services.InitializeMartenWith<InitialProductData>();
    /// </summary>
    [Fact]
    public async Task Get_All_Product()
    {
        // Act
        PageResult<ProductDto>? pagedProducts = await _albaHost.GetAsJson<PageResult<ProductDto>?>("/api/Product"); // Default page size is 20

        // Assert
        Assert.NotNull(pagedProducts);
        Assert.Equal(InitialProductData.InitialProductCount, pagedProducts.TotalCount);
        Assert.Equal(InitialProductData.InitialProductCount, pagedProducts.Items.Count());
    }

    [Fact]
    public async Task Create_and_Get_Product_LikeNormally()
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

    [Fact]
    public async Task Create_Product_TrackedSession()
    {
        // Arrange
        string location = string.Empty;

        // Act
        ITrackedSession trackedSession = await _albaHost.ExecuteAndWaitAsync(async () => location = await assumeProductCreated(_createProduct));

        // Assert
        Assert.NotEmpty(location);

        int productId = getProductIdFromLocation(location);

        //var productCreated = trackedSession.Sent.SingleMessage<ProductCreated>();
        //var productCreated = trackedSession.Executed.SingleMessage<ProductCreated>();

        var productCreated = trackedSession.FindSingleTrackedMessageOfType<ProductCreated>();

        Assert.Equal(productId, productCreated.Id);

        await using IQuerySession querySession = _documentStore.QuerySession();

        Product? storedProduct = await querySession.LoadAsync<Product>(productId);

        Assert.NotNull(storedProduct);
    }

    [Fact]
    public async Task Invoke_ProductCreated()
    {
        // Act: Invoke a handler
        ITrackedSession trackedSession = await _albaHost.InvokeMessageAndWaitAsync(ProductCreated.FromId(1));
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

    private static int getProductIdFromLocation(ReadOnlySpan<char> location)
    {
        ReadOnlySpan<char> idText = location[(location.LastIndexOf('/') + 1)..];

        return int.Parse(idText);
    }
}