﻿using Marten;
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
    //    IDocumentSession documentSession,
    //    CancellationToken cancellation)
    //{
    //    Product? product = await documentSession.LoadAsync<Product>(productLookup.Id, cancellation);

    //    _logger.LogInformation("Is Product found = {answer}", product is not null);

    //    IResult result = product is { IsDeleted: false } ? WolverineContinue.Result() : TypedResults.NotFound();

    //    return (result, product);
    //}

    public async Task<(HandlerContinuation, Product?)> BeforeAsync(
        IProductLookup productLookup,
        IDocumentSession documentSession,
        CancellationToken cancellation)
    {
        Product? product = await documentSession.LoadAsync<Product>(productLookup.Id, cancellation);

        _logger.LogInformation("Is Product found = {answer}", product is not null);

        // When you stop it, your handler won't be reached, and the HTTP response will be 'OK'. Would be better NotFound, like abowe.
        var continuation = product is { IsDeleted: false } ? HandlerContinuation.Continue : HandlerContinuation.Stop;

        return (continuation, product);
    }
}
