namespace EventSourcingApi.Entities;

public sealed record NotifyUsersCounterClosed(HashSet<Guid> UserIds);
