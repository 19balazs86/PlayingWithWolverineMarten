namespace EventSourcingApi.EventSourcing;

public record UserEvent(Guid IniciatedByUserId);

public sealed record CounterStarted(int InitialCount = 0) : UserEvent(CounterFactory.GetRandomUser());

public sealed record CounterIncreased(int Number) : UserEvent(CounterFactory.GetRandomUser());

public sealed record CounterDecreased(int Number) : UserEvent(CounterFactory.GetRandomUser());

public sealed record CounterDoNothing(int Number = 0) : UserEvent(CounterFactory.GetRandomUser());

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

    public static Guid GetRandomUser()
    {
        return Guid.Parse(_userIds[Random.Shared.Next(_userIds.Count)]);
    }

    private static readonly IReadOnlyList<string> _userIds = new List<string>
    {
        "15b0144d-b115-48c9-b18b-e4aaa259b6d1",
        "343ec12a-e884-4398-877c-9693a96eb1b0",
        "2d597888-c73b-4382-a53c-40d532f47ab1",
        "6c5d7e24-9e27-478c-8fa9-092d7f839f4b",
        "68211f27-8a31-48a0-93a5-92cdc90d630d"
    };
}