using Shared;

namespace Worker.Ping.Handlers;

public static class PongMessageHandler
{
    public static void Handle(PongMessage pong, ILogger logger)
    {
        logger.LogInformation("Got pong: {counter}", pong.Counter);
    }
}
