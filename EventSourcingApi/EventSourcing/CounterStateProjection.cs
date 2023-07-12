using Marten;
using Marten.Events;
using Marten.Events.Aggregation;

namespace EventSourcingApi.EventSourcing;

public sealed class CounterStateProjection : SingleStreamProjection<CounterState>
{
    public CounterStateProjection()
    {
        // You an use a base class or an interface type
        // IncludeType<UserEvent>();

        IncludeType<CounterStarted>();
        IncludeType<CounterIncreased>();
        IncludeType<CounterDecreased>();
        IncludeType<CounterDoNothing>();
        IncludeType<CounterClosed>();

        // You can assign a method to handle a specific event
        // Or simply go with method naming conventions
        CreateEvent<IEvent<CounterStarted>>(create);

        ProjectEvent<CounterIncreased>(applyCounterIncreased);
        ProjectEvent<CounterDecreased>(applyCounterDecreased);
        ProjectEventAsync<CounterDoNothing>(applyCounterDoNothingAsync);
        ProjectEvent<CounterClosed>(state => state.IsClosed = true);

        // This only deletes the CounterState record from the DB, but does not delete the related event records
        //DeleteEvent<CounterClosed>();
    }

    private static CounterState create(IEvent<CounterStarted> startEvent)
    {
        Guid ownerUserId = startEvent.Data.IniciatedByUserId;

        var counterState = new CounterState
        {
            Id          = startEvent.StreamId,
            Counter     = startEvent.Data.InitialCount,
            OwnerUserId = ownerUserId
        };

        counterState.SentEventByUserIds.Add(ownerUserId);

        return counterState;
    }

    private static void applyCounterIncreased(CounterState current, CounterIncreased increment)
    {
        current.Counter += increment.Number;

        current.SentEventByUserIds.Add(increment.IniciatedByUserId);
    }

    private static void applyCounterDecreased(CounterState current, CounterDecreased decrement)
    {
        current.Counter -= decrement.Number;

        current.SentEventByUserIds.Add(decrement.IniciatedByUserId);
    }

    private static async Task applyCounterDoNothingAsync(IQuerySession querySession, CounterState current, CounterDoNothing nothing)
    {
        // Simulate DB query for IQuerySession
        await Task.Delay(1_000);
    }

    // By method convention name | Static method does not apply the event, it has to be non static in order to work
    // You can use a record type as projectsion state, modify it and return with the new state
    //public CounterState Apply(CounterIncreased increment, CounterState current)
    //{
    //    long counter = current.Counter + increment.Number;

    //    return current with { Counter = counter };
    //}
}
