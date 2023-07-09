using Marten;
using Marten.Events;
using Marten.Events.Projections;

namespace EventSourcingApi.EventSourcing;

// This projection retrieves all the events from all streams.
// Because no any filters with IncludeType<..> from base class.
// This is more suitable when you do not want to group the events by stream and do not require the aggregated state.

// In the 'CustomProjection' base class, you can use CustomGrouping(new ByStreamId<TDoc>()), which apply EventSlicer by StreamId
// https://martendb.io/events/projections/custom-aggregates.html
public sealed class Custom_Async_Projection : IProjection
{
    // This can be an external service like SignalR
    // https://github.com/oskardudycz/EventSourcing.NetCore/blob/main/Sample/Helpdesk/Helpdesk.Api/Core/SignalR/SignalRProducer.cs
    private readonly TextWriter _textWriter;

    public Custom_Async_Projection(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken cancellation)
    {
        IEvent[] events = streams.SelectMany(sa => sa.Events).ToArray();

        await _textWriter.WriteLineAsync($"Start - Custom_Async_Projection | Count: {events.Length}");

        if (Random.Shared.NextDouble() <= 0.05)
        {
            throw new RankException("I am just an intermittent exception");
        }

        IGrouping<Type, IEvent>[] groups = events.GroupBy(e => e.EventType).ToArray();

        foreach (IGrouping<Type, IEvent> group in groups)
        {
            await _textWriter.WriteLineAsync($"{group.Key.Name} has {group.Count()} events");

            await Task.Delay(500);
        }

        await _textWriter.WriteLineAsync("End - Custom_Async_Projection");
    }

    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        // This projection is defined as async, no need 'Apply' for inline

        throw new NotImplementedException();
    }
}
