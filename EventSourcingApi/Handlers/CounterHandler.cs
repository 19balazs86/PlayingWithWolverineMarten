using EventSourcingApi.Entities;
using EventSourcingApi.EventSourcing;
using Marten;
using Marten.Events;
using Marten.Schema.Identity;

namespace EventSourcingApi.Handlers;

public static class CounterHandler
{
    public static async Task<Guid> Handle(CounterStartRequest startRequest, IDocumentSession documentSession)
    {
        Guid streamId = startRequest.Id ?? CombGuidIdGeneration.NewGuid();

        var counterStarted = new CounterStarted(startRequest.InitialCount);

        documentSession.Events.StartStream<CounterState>(streamId, counterStarted);

        await documentSession.SaveChangesAsync();

        return streamId;
    }

    public static async Task<long> Handle(CounterEventRequest eventRequest, IDocumentSession documentSession)
    {
        object counterEvent = CounterFactory.CreateEvent(eventRequest.Number);

        StreamAction streamAction = documentSession.Events.Append(eventRequest.Id, counterEvent);

        await documentSession.SaveChangesAsync();

        return streamAction.Version;
    }

    public static async Task Handle(CounterParalelEventRequest eventRequest, IDocumentStore documentStore, CancellationToken cancelToken)
    {
        var tasks = new List<Task>();

        // Simulate multiple event-stream operation
        foreach (Guid counterId in eventRequest.Ids)
        {
            // Simulate operations for the same event-stream
            Task streamTask = Parallel.ForEachAsync(eventRequest.Numbers, cancelToken, (number, cancel) =>
                addEvent(documentStore, counterId, number, cancel));

            tasks.Add(streamTask);
        }

        await Task.WhenAll(tasks);
    }

    private static async ValueTask addEvent(IDocumentStore documentStore, Guid streamId, int number, CancellationToken cancellation)
    {
        using IDocumentSession documentSession = documentStore.LightweightSession();

        // Place a LOCK on the event-stream!
        // You could pass in events here too, but doing this establishes a transaction
        await documentSession.Events.AppendExclusive(streamId, cancellation);

        object counterEvent = CounterFactory.CreateEvent(number);

        documentSession.Events.Append(streamId, counterEvent);

        // This will commit changes and release the lock on the event-stream
        await documentSession.SaveChangesAsync(cancellation);
    }
}
