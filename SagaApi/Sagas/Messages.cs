using JasperFx.Core;
using Wolverine;

namespace SagaApi.Sagas;

public sealed record EmailConfirmation_Start(string Email);

public sealed record EmailConfirmation_Timeout(Guid Id) : TimeoutMessage(30.Seconds());

public sealed record ConfirmEmailBySagaId(Guid Id);

public sealed record EmailConfirmed(string Email);