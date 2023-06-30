using JasperFx.Core;
using Lamar;
using Oakton;
using Oakton.Resources;
using Shared;
using Wolverine;
using Wolverine.RabbitMQ;
using Worker.Ping.Messages;

namespace Worker.Ping;

public static class Program
{
    private const string _serviceName = "Pinger";

    public static async Task<int> Main(string[] args)
    {
        IHostBuilder hostBuilder = Host
            .CreateDefaultBuilder(args)
            .UseWolverine(configureWolverine);

        // It can be run in the normal way, but this provides certain features
        // https://wolverine.netlify.app/guide/command-line.html
        return await hostBuilder.RunOaktonCommands(args);
    }

    private static void configureWolverine(HostBuilderContext context, WolverineOptions options)
    {
        options.ServiceName = _serviceName;

        options.UseRabbitMq()
            .AutoProvision()
            .AutoPurgeOnStartup();

        options.PublishMessage<PingMessage>()
            .ToRabbitQueue("wolverine-test.ping", queue => queue.TimeToLive(15.Seconds())); // Disregard any messages older than 15 seconds

        options.ListenToRabbitQueue("wolverine-test.pong");

        options.PublishMessage<RequestMessage>()
            .ToRabbitQueue("wolverine-test.request");

        // Configuring local-queues: https://wolverine.netlify.app/guide/messaging/transports/local.html#configuring-local-queues
        options.LocalQueueFor<LocalPingMessage>() // options.PublishMessage<LocalPingMessage>().Locally().MaximumParallelMessages(2);
            .MaximumParallelMessages(2);

        // This is very important to use Lamar.ServiceRegistry for the workers
        options.Services.configureLamarServices();
    }

    private static void configureLamarServices(this ServiceRegistry services)
    {
        // TODO: Uncomment a worker to test

        services.AddHostedService<Workers.LocalPingWorker>();
        //services.AddHostedService<Workers.PingWorker>();
        //services.AddHostedService<Workers.RequestWorker>();

        services.AddResourceSetupOnStartup();

        //services
        //    .AddOpenTelemetry()
        //    .WithTracing(builder => builder
        //        .SetResourceBuilder(ResourceBuilder
        //            .CreateDefault()
        //            .AddService(_serviceName)
        //            .AddService("Wolverine")
        //            .AddService("Wolverine:Pinger"))
        //        .AddAspNetCoreInstrumentation()
        //        .AddJaegerExporter()
        //        .AddSource("Wolverine"));
    }
}