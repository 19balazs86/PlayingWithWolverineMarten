using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;
using WolverineHttpWithMarten.Entities;
using WolverineHttpWithMarten.Pagination;

namespace WolverineHttpWithMarten.Endpoints;

public static class ProductEndpoints
{
    [WolverineGet("/api/Product")]
    public static async Task<PageResult<ProductDto>> GetAll(
        IQuerySession querySession,
        CancellationToken cancellationToken,
        int? pageNumber,
        int? pageSize)
    {
        var pageQuery = PageQuery<Product, ProductDto>
            .Create(pageNumber, pageSize)
            //.Filter(p => !p.IsDeleted) // SoftDeleted is enabled
            .Sort(p => p.Id)
            .Project(Mappers.ProductToDtoProjection);

        return await querySession
            .Query<Product>()
            .PaginateAsync(pageQuery, cancellationToken);
    }

    [WolverineGet("/api/Product/{id}")]
    public static async Task<ProductDto?> GetById(int id, IQuerySession querySession, CancellationToken cancellationToken)
    {
        // This could be like: https://martendb.io/documents/querying/compiled-queries.html#querying-for-a-single-document

        return await querySession
            .Query<Product>()
            .Where(p => p.Id == id)
            .Select(Mappers.ProductToDtoProjection) // You can use ProjectToType<ProductDto> method from Mapster
            .FirstOrDefaultAsync(cancellationToken);
    }

    [Transactional] // This can be omitted if you use Policies.AutoApplyTransactions
    [WolverinePost("/api/Product")]
    public static async Task<Created<Product>> Create(CreateProduct createProduct, IDocumentSession documentSession, IMessageBus messageBus)
    {
        Product product = createProduct.ToProduct();

        // Register the new document with Marten. Does not go out until Marten transaction succeeds
        documentSession.Insert(product);

        // Raise an event to be processed later
        await messageBus.SendAsync(ProductCreated.FromId(product.Id));

        // [Transactional] / Policies.AutoApplyTransactions middleware is calling the Marten IDocumentSession.SaveChangesAsync()
        //await documentSession.SaveChangesAsync();

        return TypedResults.Created($"/api/Product/{product.Id}", product);
    }

    [Transactional]
    [WolverinePost("/api/Product2")]
    public static (Created<Product>, ProductCreated) Create2(CreateProduct createProduct, IDocumentSession documentSession)
    {
        Product product = createProduct.ToProduct();

        documentSession.Insert(product);

        var response = TypedResults.Created($"/api/Product/{product.Id}", product);

        // For return type, you can use IEnumerable<object> or OutgoingMessages, but the Http response will be "No Content"
        // These 2 options are more for cascading messages, then define Http response
        return (response, ProductCreated.FromId(product.Id));
    }

    [Transactional]
    [WolverinePut("/api/Product")]
    [EmptyResponse] // Return with 204 No Content response
    public static ProductUpdated Update(UpdateProduct updateProduct, Product product, IDocumentSession documentSession)
    {
        // Note: ProductLookupMiddleware is called before this handler, and it provides the Product

        updateProduct.ApplyChangesOnProduct(product);

        documentSession.Update(product);

        return ProductUpdated.FromId(product.Id);
    }

    [WolverineDelete("/api/Product/{id}")]
    public static async Task Delete(int id, IDocumentSession documentSession)
    {
        if (Random.Shared.NextDouble() < 0.2) // Note: AddProblemDetails() to return with return with JSON.
            throw new Exception($"Random error during deleting the product({id})");

        documentSession.Delete<Product>(id); // SoftDeleted is enabled
        //documentSession.HardDelete<Product>(id);

        // Call it manually, becase there is no [Transactional] / Policies.AutoApplyTransactions middleware included.
        await documentSession.SaveChangesAsync();
    }
}
