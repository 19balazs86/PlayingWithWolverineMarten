using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using Wolverine.Http;
using WolverineHttpWebAPI.Services;

namespace WolverineHttpWebAPI.Endpoints;

public static class HelloEndpoints
{
    [WolverineGet("/", Name = "Redirect to Swagger")]
    public static RedirectHttpResult Get() => TypedResults.Redirect("/swagger");

    // Note: In the generated Wolverine HTTP handler, there is a new ScopedTestService class created
    // It creates a new object even if you mark it FromServices
    // Getting services from DI container use the IServiceProvider
    [WolverineGet("/TestScopeLifetime")]
    public static string TestScopeLifetime([FromServices] ScopedTestService testService1, IServiceProvider serviceProvider)
    {
        var testService2 = serviceProvider.GetRequiredService<ScopedTestService>();

        return $"{testService1.GetScopeId()}\r\n{testService2.GetScopeId()}";
    }

    // This is not Wolverine related
    public static async Task PageNotFound(HttpContext context)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode  = StatusCodes.Status404NotFound;

        var problemDetails = new ProblemDetails
        {
            Title  = "The requested endpoint is not found.",
            Status = StatusCodes.Status404NotFound,
        };

        await JsonSerializer.SerializeAsync(context.Response.Body, problemDetails);
    }
}
