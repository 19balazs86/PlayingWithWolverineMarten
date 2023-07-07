﻿using EventSourcingApi.Entities;
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

    // Get the aggregation and check it, before to send any events
    public static async Task<long> Handle(CounterEventCheckRequest eventRequest, IDocumentSession documentSession)
    {
        object counterEvent = CounterFactory.CreateEvent(eventRequest.Number);

        // Fetch the projected aggregate with built-in optimistic concurrency checks
        IEventStream<CounterState> eventStream = await documentSession.Events.FetchForWriting<CounterState>(eventRequest.Id);

        CounterState counterState = eventStream.Aggregate;

        long newCounter = counterState.Counter + eventRequest.Number;

        // Counter should between [-500, 500]
        // You can check it before to send any events
        // Or let the message go and use this check in the projection
        if ((eventRequest.Number > 0 && newCounter > 500) || (eventRequest.Number < 0 && newCounter < -500))
            return -1;

        eventStream.AppendOne(counterEvent);

        await documentSession.SaveChangesAsync();

        return eventStream.CurrentVersion.GetValueOrDefault();
    }

    public static async Task Handle(CounterParalelEventRequest eventRequest, IDocumentStore documentStore, CancellationToken cancelToken)
    {
        var tasks = new List<Task>();

        // Simulate the operation of multiple event streams
        foreach (Guid counterId in eventRequest.Ids)
        {
            // Simulate operations for the same event-stream
            Task streamTask = Parallel.ForEachAsync(eventRequest.Numbers, cancelToken, (number, cancel) =>
            {
                return appendEvent(documentStore, counterId, number, cancel);
            });

            tasks.Add(streamTask);
        }

        await Task.WhenAll(tasks);
    }

    private static async ValueTask appendEvent(IDocumentStore documentStore, Guid streamId, int number, CancellationToken cancellation)
    {
        using IDocumentSession documentSession = documentStore.LightweightSession();

        // This method establishes a transaction with exclusive LOCK against the stream
        // You could pass in any events as well
        await documentSession.Events.AppendExclusive(streamId, cancellation);

        // Same as above, but return the current state to facilitate decision-making if necessary
        // IEventStream<CounterState> eventStream = await documentSession.Events.FetchForExclusiveWriting<CounterState>(streamId, cancellation);

        object counterEvent = CounterFactory.CreateEvent(number);

        documentSession.Events.Append(streamId, counterEvent);

        // This will commit changes and release the lock on the event-stream
        await documentSession.SaveChangesAsync(cancellation);
    }
}
