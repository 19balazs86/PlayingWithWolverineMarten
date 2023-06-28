using JasperFx.Core;
using Shared;
using Wolverine;

namespace Worker.Pong.Handlers;

public static class RequestMessageHandler
{
    public static async Task Handle(RequestMessage request, ILogger logger, IMessageContext messageContext)
    {
        logger.LogInformation("Got request: {counter}", request.Counter);

        if (Random.Shared.NextDouble() <= 0.05)
            throw new Exception("Random exception, can not process Request message");

        await Task.Delay(1.Seconds());

        var response = new ResponseMessage(request.Counter);

        // RequestMessage sent with DeliveryOptions.RequireResponse
        // There is no response queue defined in the Program.cs
        // It will be sent back via caller response-queue like: "wolverine.response.-655612791", which is created by default
        await messageContext.RespondToSenderAsync(response);
    }
}
