using JasperFx.Core;
using Wolverine;
using Worker.Ping.Messages;

namespace Worker.Ping.Workers;

public sealed class LocalPingWorker : BackgroundService
{
    private readonly ILogger<LocalPingWorker> _logger;
    private readonly IMessageBus _messageBus;

    private int _counter = 1;

    public LocalPingWorker(ILogger<LocalPingWorker> logger, IMessageBus messageBus)
    {
        _logger     = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ping = new LocalPingMessage(_counter++);

            //InnerPongMessage pong = await _messageBus.InvokeAsync<InnerPongMessage>(ping, stoppingToken); // Wait for the handler to process the message

            _logger.LogInformation("Sending ping: {counter}", ping.Counter);

            await _messageBus.SendAsync(ping);

            await Task.Delay(500.Milliseconds());
        }
    }
}
