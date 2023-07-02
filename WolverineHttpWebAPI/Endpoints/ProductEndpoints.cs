using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;
using WolverineHttpWebAPI.Entities;
using WolverineHttpWebAPI.Pagination;

namespace WolverineHttpWebAPI.Endpoints;

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

    [WolverinePut("/api/Product")]
    public static async Task<Ok<UpdateProduct>> Update(UpdateProduct updateProduct, Product product, IDocumentSession documentSession)
    {
        // Note: ProductLookupMiddleware is called before this handler, and it provides the Product

        updateProduct.ApplyChangesOnProduct(product);

        documentSession.Update(product);

        await documentSession.SaveChangesAsync();

        // There is an issue with wolverine
        // A local variable or function named 'result' is already defined in this scope
        // In this method, can not return with IResult or Task.
        // TODO: After the bug fixed, return with Task.
        return TypedResults.Ok(updateProduct);
    }

    [WolverineDelete("/api/Product/{id}")]
    public static async Task Delete(int id, IDocumentSession documentSession)
    {
        // Note: ProductLookupMiddleware is called before this handler, and it provides the Product

        if (Random.Shared.NextDouble() < 0.2) // Note: AddProblemDetails() to make it in JSON format.
            throw new Exception($"Random error during deleting the product({id})");

        documentSession.Delete<Product>(id); // SoftDeleted is enabled
        //documentSession.HardDelete<Product>(id);

        // Call it manually, becase there is no [Transactional] / Policies.AutoApplyTransactions middleware included.
        await documentSession.SaveChangesAsync();
    }
}
