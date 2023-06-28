using JasperFx.Core;
using Shared;

namespace Worker.Pong.Handlers;

public static class PingMessageHandler
{
    public static async Task<PongMessage> Handle(PingMessage ping, ILogger logger)
    {
        logger.LogInformation("Got ping: {counter}", ping.Counter);

        if (Random.Shared.NextDouble() <= 0.05)
            throw new Exception("Random exception, can not process Ping message");

        await Task.Delay(1.Seconds());

        // Simply return and it will know its route to the response queue
        return new PongMessage(ping.Counter);

        // See the RequestMessageHandler
        //await _messageContext.RespondToSenderAsync(pong);

        // The response queue is defined by: PublishMessage<PongMessage>().ToRabbitQueue("wolverine-test.pong");
        //await _messageContext.SendAsync(pong);
    }
}
