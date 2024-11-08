using JasperFx.Core;
using Wolverine;
using Worker.Ping.Messages;

namespace Worker.Ping.Workers;

public sealed class LocalPingWorker(ILogger<LocalPingWorker> _logger, IServiceScopeFactory _serviceScopeFactory) : BackgroundService
{
    private int _counter = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(500.Milliseconds());

            var ping = new LocalPingMessage(_counter++);

            //InnerPongMessage pong = await _messageBus.InvokeAsync<InnerPongMessage>(ping, stoppingToken); // Wait for the handler to process the message

            _logger.LogInformation("Sending ping: {counter}", ping.Counter);

            await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();

            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            await messageBus.SendAsync(ping);
        }
    }
}
