namespace EventSourcingApi.EventSourcing;

public sealed record CounterStarted(int InitialCount = 0);

public sealed record CounterIncreased(int Number);

public sealed record CounterDecreased(int Number);

public sealed record CounterDoNothing(int Number = 0);

// Version is set automatically by Marten if used as the target of a SingleStreamAggregation
public sealed record CounterState(Guid Id, long Counter, long Version = 0);

public static class CounterFactory
{
    public static object CreateEvent(int number)
    {
        return number switch
        {
            0   => new CounterDoNothing(0),
            < 0 => new CounterDecreased(Math.Abs(number)),
            _   => new CounterIncreased(number),
        };
    }
}