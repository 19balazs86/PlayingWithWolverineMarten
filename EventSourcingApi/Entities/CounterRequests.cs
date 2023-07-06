namespace EventSourcingApi.Entities;

public sealed record CounterStartRequest(Guid? Id, int InitialCount = 0);

public sealed record CounterEventRequest(Guid Id, int Number);

public sealed record CounterParalelEventRequest(Guid[] Ids, int[] Numbers);