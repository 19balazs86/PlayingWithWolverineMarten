using JasperFx.Core;
using Shared;
using Wolverine;

namespace Worker.Ping.Workers;

public sealed class PingWorker(IServiceScopeFactory _serviceScopeFactory) : BackgroundService
{
    private int _counter = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(500.Milliseconds());

            var ping = new PingMessage(_counter++);

            await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();

            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            await messageBus.SendAsync(ping);
        }
    }
}
