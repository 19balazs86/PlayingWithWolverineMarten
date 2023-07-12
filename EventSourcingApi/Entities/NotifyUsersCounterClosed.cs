namespace EventSourcingApi.Entities;

public sealed record NotifyUsersCounterClosed(IEnumerable<Guid> UserIds);
