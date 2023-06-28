using JasperFx.Core;
using Shared;
using Wolverine;

namespace Worker.Ping.Workers;

public sealed class PingWorker : BackgroundService
{
    private readonly IMessageBus _messageBus;

    private int _counter = 1;

    public PingWorker(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var ping = new PingMessage(_counter++);

            await _messageBus.SendAsync(ping);

            await Task.Delay(500.Milliseconds());
        }
    }
}
