using Shared;

namespace Worker.Ping.Handlers;

public static class ResponseMessageHandler
{
    public static void Handle(ResponseMessage responseMessage, ILogger logger)
    {
        logger.LogInformation("Got response: {counter}", responseMessage.Counter);
    }
}
