namespace EventSourcingApi.Entities;

public sealed record CounterStartRequest(int InitialCount = 0);

public sealed record CounterEventRequest(Guid Id, int Number);

public sealed record CounterParalelEventRequest(Guid Id);