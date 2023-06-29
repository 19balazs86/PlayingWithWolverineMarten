using Marten;
using Wolverine.Http;
using WolverineHttpWebAPI.Entities;

namespace WolverineHttpWebAPI.Middlewares;

// You can make the middleware static and pass the ILogger in method parameter, which is good.
// But in that case the ILogger will be ILogger<HandlerType>. It is not bad...
public class ProductLookupMiddleware
{
    private readonly ILogger<ProductLookupMiddleware> _logger;

    public ProductLookupMiddleware(ILogger<ProductLookupMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task<(IResult, Product?)> BeforeAsync(
        IProductLookup productLookup,
        IDocumentSession documentSession,
        CancellationToken cancellation)
    {
        Product? product = await documentSession.LoadAsync<Product>(productLookup.Id, cancellation);

        _logger.LogInformation("Is Product found = {answer}", product is not null);

        IResult result = product is null ? TypedResults.NotFound() : WolverineContinue.Result();

        return (result, product);
    }

    //public async Task<(HandlerContinuation, Product?)> BeforeAsync(
    //    IProductLookup productLookup,
    //    IDocumentSession documentSession,
    //    CancellationToken cancellation)
    //{
    //    Product? product = await documentSession.LoadAsync<Product>(productLookup.Id, cancellation);

    //    _logger.LogInformation("Is Product found = {answer}", product is not null);

    //    // When you stop it, the Handler won't be reached, and the HTTP response will be 'OK'.
    //    var continuation = product is null ? HandlerContinuation.Stop : HandlerContinuation.Continue;

    //    return (continuation, product);
    //}
}
