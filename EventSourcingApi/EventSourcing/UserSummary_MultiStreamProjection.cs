using Marten.Events.Projections;

namespace EventSourcingApi.EventSourcing;

public sealed class UserSummary_MultiStreamProjection : MultiStreamProjection<UserSummary, Guid>
{
    public UserSummary_MultiStreamProjection()
    {
        ProjectionName = nameof(UserSummary); // This does not change anything. By default, the name is generated with the aggregation type

        // Optional, but important optimization for the async daemon
        // so that it sets up an allow list of the event types that will be run through this projection
        IncludeType<UserEvent>();

        // Specifying the aggregate document id per event type
        // Identity<CounterStarted>(x => x.IniciatedByUserId);
        // Identity<CounterIncreased>(x => x.IniciatedByUserId);
        // ...

        // Using a common interface or base type, and specify the identity rule on that common type
        Identity<UserEvent>(x => x.IniciatedByUserId);

        ProjectEvent<CounterStarted>  (summary => summary.StartEventCount++);
        ProjectEvent<CounterIncreased>(summary => summary.IncreaseEventCount++);
        ProjectEvent<CounterDecreased>(summary => summary.DecreaseEventCount++);
        ProjectEvent<CounterDoNothing>(summary => summary.DoNothingEventCount++);
    }
}
