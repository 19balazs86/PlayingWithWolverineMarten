using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text.Json;
using Wolverine.Http;

namespace WolverineHttpWithMarten.Endpoints;

public static class HelloEndpoints
{
    [WolverineGet("/", Name = "Redirect to Swagger")]
    public static RedirectHttpResult Get() => TypedResults.Redirect("/swagger");

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
