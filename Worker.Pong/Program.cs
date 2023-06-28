using JasperFx.Core;
using Lamar;
using Oakton;
using Shared;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Worker.Pong;

public static class Program
{
    private const string _serviceName = "Ponger";

    public static async Task<int> Main(string[] args)
    {
        IHostBuilder hostBuilder = Host
            .CreateDefaultBuilder(args)
            .UseWolverine(configureWolverine);

        return await hostBuilder.RunOaktonCommands(args);
    }

    private static void configureWolverine(HostBuilderContext context, WolverineOptions options)
    {
        options.ServiceName = _serviceName;

        // options.UseMemoryPackSerialization(); https://wolverine.netlify.app/guide/messages.html#memorypack-serialization

        options.UseRabbitMq()
            .AutoProvision()
            .AutoPurgeOnStartup();

        options.PublishMessage<PongMessage>()
            .ToRabbitQueue("wolverine-test.pong");

        options.ListenToRabbitQueue("wolverine-test.ping", queue => queue.TimeToLive(15.Seconds())) // Disregard any messages older than 15 seconds
            .PreFetchCount(5)
            .ListenerCount(3); // Use 3 parallel listeners

        options.ListenToRabbitQueue("wolverine-test.request")
            .PreFetchCount(5)
            .ListenerCount(3);

        // This is very important to use Lamar.ServiceRegistry
        options.Services.configureLamarServices();
    }

    private static void configureLamarServices(this ServiceRegistry services)
    {

    }
}