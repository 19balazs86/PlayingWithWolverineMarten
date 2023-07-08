using Marten.Exceptions;
using SagaApi.Sagas;
using Wolverine;
using Wolverine.Persistence.Sagas;

namespace SagaApi;

public static class Endpoints
{
    public static async Task<IResult> EmailConfirmation_Start(string email, IMessageBus messageBus)
    {
        var start = new EmailConfirmation_Start(email);

        try
        {
            // Invoke method can not return with the saga state
            // But can return with a cascaded message
            var timeout = await messageBus.InvokeAsync<EmailConfirmation_Timeout>(start);

            return TypedResults.Ok(timeout.Id);
        }
        catch (DocumentAlreadyExistsException)
        {
            // Due to the UniqueIndex(x => x.Email), user can not initiate registration with the same email

            return TypedResults.BadRequest($"Registration is already started with '{email}'");
        }
    }

    public static async Task<IResult> EmailConfirmation_Confirm(Guid sagaId, IMessageBus messageBus)
    {
        var confirm = new ConfirmEmailBySagaId(sagaId);

        try
        {
            await messageBus.InvokeAsync(confirm);

            return TypedResults.Ok();
        }
        catch (UnknownSagaException)
        {
            return TypedResults.BadRequest("No registration found with the given Id");
        }
    }
}
