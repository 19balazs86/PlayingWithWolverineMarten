namespace WolverineHttpWithMarten.UnitTest;

/// <summary>
/// This is an example of unit testing our work and mocking 2 main components:
/// 1) IMessageBus or IMessageContext for Wolverine
/// 2) IDocumentSession for Marten
///
/// Documentation: https://wolverine.netlify.app/guide/testing.html
/// </summary>
public sealed class ProductEndpointsTests
{
    private readonly Mock<IDocumentSession> _documentSessionMock;

    public ProductEndpointsTests()
    {
        _documentSessionMock = new Mock<IDocumentSession>(MockBehavior.Strict);
    }

    [Fact]
    public async Task CreateProduct_Ok()
    {
        // Arrange
        var createProduct = new CreateProduct
        {
            Name         = "ProductName",
            Price        = 50,
            Description  = "ProductDescription",
            CategoryEnum = CategoryEnum.Category1
        };

        var testMessageContext = new TestMessageContext();

        _documentSessionMock
            .Setup(x => x.Insert(It.IsAny<Product>()))
            .Verifiable();

        // Act
        Created<Product> response = await ProductEndpoints.Create(createProduct, _documentSessionMock.Object, testMessageContext);

        Product? product = response?.Value;

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(product);

        Assert.Equal(1, testMessageContext.Sent.Count);

        var productCreated = testMessageContext.Sent.ShouldHaveMessageOfType<ProductCreated>();

        Assert.NotNull(productCreated);
        Assert.Equal(product.Id, productCreated.Id); // This is obviously 0

        _documentSessionMock.Verify(x => x.Insert(It.Is<Product>(p => p == product)), Times.Once());

        // IDocumentSession.SaveChangesAsync is called in the tranzactional middleware, no need to verify
    }
}