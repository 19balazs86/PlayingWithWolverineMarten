using EventSourcingApi.Entities;
using EventSourcingApi.EventSourcing;
using Marten;
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
}
