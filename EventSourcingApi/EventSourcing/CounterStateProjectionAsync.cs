using Marten;
using Marten.Events;
using Marten.Events.Projections;

namespace EventSourcingApi.EventSourcing;

public sealed class CounterStateProjectionAsync : IProjection
{
    // This can be any external service like SignalR
    // https://github.com/oskardudycz/EventSourcing.NetCore/blob/main/Sample/Helpdesk/Helpdesk.Api/Core/SignalR/SignalRProducer.cs
    private readonly TextWriter _textWriter;

    public CounterStateProjectionAsync(TextWriter textWriter)
    {
        _textWriter = textWriter;
    }

    public async Task ApplyAsync(IDocumentOperations operations, IReadOnlyList<StreamAction> streams, CancellationToken cancellation)
    {
        IEvent[] events = streams.SelectMany(sa => sa.Events).ToArray();

        IGrouping<Type, IEvent>[] groups = events.GroupBy(e => e.EventType).ToArray();

        await _textWriter.WriteLineAsync($"Start - CounterStateProjectionAsync | Count: {events.Length}");

        foreach (IGrouping<Type, IEvent> group in groups)
        {
            await _textWriter.WriteLineAsync($"{group.Key.Name} has {group.Count()} elements");

            await Task.Delay(500);
        }

        await _textWriter.WriteLineAsync("End - CounterStateProjectionAsync");
    }

    public void Apply(IDocumentOperations operations, IReadOnlyList<StreamAction> streams)
    {
        throw new NotImplementedException();
    }
}
