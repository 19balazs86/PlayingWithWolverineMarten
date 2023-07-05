using EventSourcingApi.Endpoints;
using EventSourcingApi.EventSourcing;
using Lamar;
using Marten;
using Marten.Events.Projections;
using Marten.Services.Json;
using Oakton;
using Oakton.Resources;
using Weasel.Core;
using Wolverine;
using Wolverine.Http;
using Wolverine.Marten;

namespace EventSourcingApi;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;
        IConfiguration configuration  = builder.Configuration;
        IHostBuilder hostBuilder      = builder.Host;

        // Add services to the container
        {
            hostBuilder.ApplyOaktonExtensions();

            hostBuilder.UseWolverine(configureWolverine);

            services
                .AddMarten(options => configureMarten(options, configuration))
                .UseLightweightSessions()
                .IntegrateWithWolverine();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.MapWolverineEndpoints();

            app.MapCounterEndpoints();
        }

        return await app.RunOaktonCommands(args);
    }

    private static void configureWolverine(HostBuilderContext context, WolverineOptions options)
    {
        options.Services.configureLamarServices();
    }

    private static void configureMarten(StoreOptions options, IConfiguration configuration)
    {
        string postgreSqlConnString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new NullReferenceException("Missing PostgreSQL connection string");

        options.Connection(postgreSqlConnString);

        options.UseDefaultSerialization(serializerType: SerializerType.SystemTextJson, enumStorage: EnumStorage.AsString);

        options.Projections.Add<CounterStateProjection>(ProjectionLifecycle.Inline);
    }

    private static void configureLamarServices(this ServiceRegistry services)
    {
        services.AddResourceSetupOnStartup();
    }
}