namespace EventSourcingApi.EventSourcing;

public sealed record CounterStarted(int InitialCount = 0);

public sealed record CounterIncreased(int Number);

public sealed record CounterDecreased(int Number);

// Version is set automatically by Marten if used as the target of a SingleStreamAggregation
public sealed record CounterState(Guid Id, long Counter, long Version = 0);

public static class CounterFactory
{
    public static object CreateEvent(int number)
    {
        if (number < 0)
        {
            return new CounterDecreased(Math.Abs(number));
        }

        return new CounterIncreased(number);
    }
}