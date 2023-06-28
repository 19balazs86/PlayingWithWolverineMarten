using JasperFx.Core;
using Wolverine;
using Wolverine.Attributes;
using Worker.Ping.Messages;

namespace Worker.Ping.Handlers;

public sealed class Local_Ping_Pong_MessageHandler
{
    private readonly ILogger<Local_Ping_Pong_MessageHandler> _logger;

    public Local_Ping_Pong_MessageHandler(ILogger<Local_Ping_Pong_MessageHandler> logger)
    {
        _logger = logger;
    }

    // Error handling
    // https://wolverine.netlify.app/guide/handlers/error-handling.html#scoping
    [RetryNow(typeof(Exception), 500, 1_000)]
    public async Task<LocalPongMessage> Handle(LocalPingMessage ping, CancellationToken cancellation)
    {
        _logger.LogInformation("Got ping: {counter}", ping.Counter);

        if (Random.Shared.NextDouble() <= 0.05)
            throw new Exception("Random exception, can not process Ping message");

        await Task.Delay(1.Seconds(), cancellation);

        return new LocalPongMessage(ping.Counter);
    }

    // You can inject Envelope for metadata about the current message
    // https://wolverine.netlify.app/guide/handlers/#message-handler-parameters
    public void Handle(LocalPongMessage pong, Envelope envelope)
    {
        _logger.LogInformation("Got pong: {counter}", pong.Counter);
    }
}
