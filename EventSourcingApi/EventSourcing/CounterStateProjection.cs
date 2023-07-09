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

        // You can assign a method to handle a specific event
        // Or simply go with method naming conventions
        CreateEvent<IEvent<CounterStarted>>(create);

        ProjectEvent<CounterIncreased>(applyCounterIncreased);
        ProjectEvent<CounterDecreased>(applyCounterDecreased);
        ProjectEventAsync<CounterDoNothing>(applyCounterDoNothingAsync);

        //DeleteEvent<>
    }

    private static CounterState create(IEvent<CounterStarted> startEvent)
    {
        return new CounterState(startEvent.StreamId, startEvent.Data.InitialCount);
    }

    private static CounterState applyCounterIncreased(CounterState current, CounterIncreased increment)
    {
        long counter = current.Counter + increment.Number;

        return current with { Counter = counter };
    }

    private static CounterState applyCounterDecreased(CounterState current, CounterDecreased decrement)
    {
        long counter = current.Counter - decrement.Number;

        return current with { Counter = counter };
    }

    private static async Task<CounterState> applyCounterDoNothingAsync(IQuerySession querySession, CounterState current, CounterDoNothing nothing)
    {
        // Simulate DB query for IQuerySession
        await Task.Delay(1_000);

        return current;
    }

    // By method convention name | Static method does not apply the event, it has to be non static in order to work
    //public CounterState Apply(CounterIncreased increment, CounterState current)
    //{
    //    long counter = current.Counter + increment.Number;

    //    return current with { Counter = counter };
    //}
}
