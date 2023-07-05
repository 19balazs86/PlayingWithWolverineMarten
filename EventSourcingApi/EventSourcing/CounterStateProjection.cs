using Marten.Events;
using Marten.Events.Aggregation;

namespace EventSourcingApi.EventSourcing;

public sealed class CounterStateProjection : SingleStreamProjection<CounterState>
{
    public CounterStateProjection()
    {
        IncludeType<CounterStarted>();
        IncludeType<CounterIncreased>();
        IncludeType<CounterDecreased>();

        // You can handle events by defining them or by using naming conventions for the method
        CreateEvent<IEvent<CounterStarted>>(create);

        ProjectEvent<CounterIncreased>(applyCounterIncreased);
        ProjectEvent<CounterDecreased>(applyCounterDecreased);

        //DeleteEventEvent<>
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

    // By method convention name | Static method does not apply the event
    //public CounterState Apply(CounterIncreased increment, CounterState current)
    //{
    //    long counter = current.Counter + increment.Number;

    //    return current with { Counter = counter };
    //}
}
