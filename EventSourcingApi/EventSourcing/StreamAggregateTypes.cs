namespace EventSourcingApi.EventSourcing;

// The version is automatically set by Marten if it is used as the target of a SingleStreamAggregation
public sealed record CounterState(Guid Id, long Counter, long Version = 0);

public sealed class UserSummary
{
    public Guid Id { get; set; } // UserId, IniciatedByUserId
    public int StartEventCount { get; set; }
    public int IncreaseEventCount { get; set; }
    public int DecreaseEventCount { get; set; }
    public int DoNothingEventCount { get; set; }
}