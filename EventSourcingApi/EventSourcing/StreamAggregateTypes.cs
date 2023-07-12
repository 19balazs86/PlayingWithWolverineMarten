namespace EventSourcingApi.EventSourcing;

public sealed class CounterState
{
    public Guid Id { get; init; }
    public long Counter { get; set; }
    public Guid OwnerUserId { get; init; } = Guid.Empty;
    public long Version { get; set; } // The version is automatically set by Marten if it is used as the target of a SingleStreamAggregation
    public bool IsClosed { get; set; }
    public HashSet<Guid> SentEventByUserIds { get; init; } = new HashSet<Guid>();
}

public sealed class UserSummary
{
    public Guid Id { get; set; } // UserId, IniciatedByUserId
    public int StartEventCount { get; set; }
    public int CloseEventCount { get; set; }
    public int IncreaseEventCount { get; set; }
    public int DecreaseEventCount { get; set; }
    public int DoNothingEventCount { get; set; }
}