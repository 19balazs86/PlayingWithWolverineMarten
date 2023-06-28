using JasperFx.Core;
using Shared;
using Wolverine;

namespace Worker.Ping.Workers;

public sealed class RequestWorker : BackgroundService
{
    private readonly ILogger<RequestWorker> _logger;
    private readonly IMessageBus _messageBus;

    private int _counter = 1;

    public RequestWorker(ILogger<RequestWorker> logger, IMessageBus messageBus)
    {
        _logger     = logger;
        _messageBus = messageBus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var request = new RequestMessage(_counter++);

            _logger.LogInformation("Sending request: {counter}", request.Counter);

            // It will be sent back via response-queue like: "wolverine.response.-655612791", which is created by default
            await _messageBus.SendAsync(request, DeliveryOptions.RequireResponse<ResponseMessage>());

            await Task.Delay(500.Milliseconds());
        }
    }
}
