using Wolverine;

namespace SagaApi.Sagas;

// More information: https://wolverine.netlify.app/guide/durability/sagas.html
public sealed class EmailConfirmationSaga : Saga
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public static (EmailConfirmationSaga, EmailConfirmation_Timeout, Guid) Start(EmailConfirmation_Start start, ILogger logger)
    {
        Guid sagaId = Guid.NewGuid();

        var saga = new EmailConfirmationSaga
        {
            Id    = sagaId,
            Email = start.Email
        };

        var timeout = new EmailConfirmation_Timeout(sagaId);

        logger.LogWarning("http://localhost:5076/confirm/{Id}", sagaId);

        // The saga state will be persisted in the DB
        // The timeout message will be cascaded
        return (saga, timeout, sagaId);
    }

    public EmailConfirmed Handle(ConfirmEmailBySagaId confirm, ILogger logger)
    {
        logger.LogInformation("Email '{email}' is confirmed", Email);

        // You can modify any fields, and it will be persisted in the database, instead of deleting it by marking as completed
        // Field1 = "Changed";

        // Delete the saga state after the message is done
        MarkCompleted();

        // Cascade a message, small work for the EmailConfirmedHandler
        return new EmailConfirmed(Email);
    }

    public void Handle(EmailConfirmation_Timeout timeout, ILogger logger)
    {
        logger.LogInformation("Timeout: '{email}'", Email);

        // Delete the saga state after the message is done
        MarkCompleted();
    }
}
