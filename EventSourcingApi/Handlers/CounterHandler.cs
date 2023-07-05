using EventSourcingApi.Entities;
using EventSourcingApi.EventSourcing;
using Marten;
using Marten.Events;

namespace EventSourcingApi.Handlers;

public static class CounterHandler
{
    public static async Task<Guid> Handle(CounterStartRequest startRequest, IDocumentSession documentSession)
    {
        Guid streamId = startRequest.Id ?? Guid.NewGuid();

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

    public static async Task Handle(CounterParalelEventRequest eventRequest, IDocumentStore documentStore)
    {
        int[] numbers = Enumerable.Range(1, 5).Select(_ => Random.Shared.Next(-100, 101)).ToArray();

        // To simulate paralell work for the same event-stream
        await Parallel.ForEachAsync(numbers, (number, _) => addEvent(documentStore, eventRequest.Id, number));
    }

    private static async ValueTask addEvent(IDocumentStore documentStore, Guid streamId, int number)
    {
        using IDocumentSession documentSession = documentStore.LightweightSession();

        // Place a LOCK on the event-stream!
        // You could pass in events here too, but doing this establishes a transaction
        await documentSession.Events.AppendExclusive(streamId);

        object counterEvent = CounterFactory.CreateEvent(number);

        documentSession.Events.Append(streamId, counterEvent);

        // This will commit changes and release the lock on the event-stream
        await documentSession.SaveChangesAsync();
    }
}
