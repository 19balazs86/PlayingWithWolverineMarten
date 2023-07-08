using SagaApi.Sagas;

namespace SagaApi.Handlers;

public static class EmailConfirmedHandler
{
    public static void Handle(EmailConfirmed confirmed, ILogger logger)
    {
        logger.LogInformation("Got a message for EmailConfirmed('{email}')", confirmed.Email);
    }
}
