using EventSourcingApi.Entities;
using EventSourcingApi.EventSourcing;
using Marten;
using Microsoft.AspNetCore.Http.HttpResults;
using Wolverine;
using Wolverine.Http;

namespace EventSourcingApi.Endpoints;

public static class CounterEndpoints
{
    public static IEndpointRouteBuilder MapCounterEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapPostToWolverine<CounterStartRequest, Guid>("/Counter/Start");

        builder.MapPutToWolverine<CounterEventRequest, long>("/Counter/Event");

        builder.MapPutToWolverine<CounterEventCheckRequest, long>("/Counter/EventCheck");

        builder.MapPutToWolverine<CounterParalelEventRequest>("/Counter/Events");

        return builder;
    }

    [WolverineGet("/Counter/{streamId}")]
    public static async Task<CounterState?> GetCounterState(Guid streamId, IQuerySession querySession)
    {
        // StreamState? streamState = await querySession.Events.FetchStreamStateAsync(streamId);

        return await querySession.LoadAsync<CounterState>(streamId);
    }

    [WolverineDelete("/Counter/Close")]
    public static async Task<Results<Ok, BadRequest<string>>> DeleteCounterClose(CounterCloseRequest closeRequest, IMessageBus messageBus)
    {
        try
        {
            await messageBus.InvokeAsync(closeRequest);

            return TypedResults.Ok();
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }
}
