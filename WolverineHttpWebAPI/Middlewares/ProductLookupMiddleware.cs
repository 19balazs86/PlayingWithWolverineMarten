using Marten;
using Wolverine;
using WolverineHttpWebAPI.Entities;

namespace WolverineHttpWebAPI.Middlewares;

// You can make the middleware static and pass the ILogger in method parameter, which is good.
// But in that case the ILogger will be ILogger<HandlerType>. It is not bad...
public sealed class ProductLookupMiddleware
{
    private readonly ILogger<ProductLookupMiddleware> _logger;

    public ProductLookupMiddleware(ILogger<ProductLookupMiddleware> logger)
    {
        _logger = logger;
    }

    // !!! Right now this Validation middleware cause an exception in case of IResult response
    //public async Task<(IResult, Product?)> BeforeAsync(
    //    IProductLookup productLookup,
    //    IQuerySession querySession,
    //    CancellationToken cancellation)
    //{
    //    // This can load the soft-deleted entities
    //    // Product? product = await querySession.LoadAsync<Product>(productLookup.Id, cancellation);

    //    Product? product = await querySession.Query<Product>().FirstOrDefaultAsync(p => p.Id == productLookup.Id, cancellation);

    //    _logger.LogInformation("Is Product found = {answer}", product is not null);

    //    IResult result = product is null ? TypedResults.NotFound() : WolverineContinue.Result();

    //    return (result, product);
    //}

    public async Task<(HandlerContinuation, Product?)> BeforeAsync(
        IProductLookup productLookup,
        IQuerySession querySession,
        CancellationToken cancellation)
    {
        // This can load the soft-deleted entities
        // Product? product = await querySession.LoadAsync<Product>(productLookup.Id, cancellation);

        Product? product = await querySession.Query<Product>().FirstOrDefaultAsync(p => p.Id == productLookup.Id, cancellation);

        _logger.LogInformation("Is Product found = {answer}", product is not null);

        // When you stop it, your handler won't be reached, and the HTTP response will be 'OK'. Would be better NotFound, like abowe.
        var continuation = product is null ? HandlerContinuation.Stop : HandlerContinuation.Continue;

        return (continuation, product);
    }
}
