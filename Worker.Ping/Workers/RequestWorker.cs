using JasperFx.Core;
using Shared;
using Wolverine;

namespace Worker.Ping.Workers;

public sealed class RequestWorker(ILogger<RequestWorker> _logger, IServiceScopeFactory _serviceScopeFactory) : BackgroundService
{
    private int _counter = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(500.Milliseconds());

            var request = new RequestMessage(_counter++);

            _logger.LogInformation("Sending request: {counter}", request.Counter);

            await using AsyncServiceScope scope = _serviceScopeFactory.CreateAsyncScope();

            var messageBus = scope.ServiceProvider.GetRequiredService<IMessageBus>();

            // It will be sent back via response-queue like: "wolverine.response.-655612791", which is created by default
            await messageBus.SendAsync(request, DeliveryOptions.RequireResponse<ResponseMessage>());
        }
    }
}
