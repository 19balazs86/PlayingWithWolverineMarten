using EventSourcingApi.Endpoints;
using EventSourcingApi.EventSourcing;
using JasperFx.Core;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Oakton;
using Oakton.Resources;
using Polly;
using Polly.Retry;
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
                .IntegrateWithWolverine()
                .AddAsyncDaemon(DaemonMode.Solo);
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.MapWolverineEndpoints();

            app.MapCounterEndpoints();
        }

        return await app.RunOaktonCommands(args);
    }

    private static void configureWolverine(WolverineOptions options)
    {
        options.Services.AddResourceSetupOnStartup();
    }

    private static void configureMarten(StoreOptions options, IConfiguration configuration)
    {
        string postgreSqlConnString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new NullReferenceException("Missing PostgreSQL connection string");

        options.Connection(postgreSqlConnString);

        options.UseSystemTextJsonForSerialization();

        options.Projections.Add<CounterStateProjection>(ProjectionLifecycle.Inline);
        options.Projections.Add(new Custom_Async_Projection(Console.Out), ProjectionLifecycle.Async);
        options.Projections.Add<UserSummary_MultiStreamProjection>(ProjectionLifecycle.Async);

        // Handle with retry the intermittent RankException in the async projection
        options.ExtendPolly(pipelineBuilder => pipelineBuilder.AddRetry(_retryStrategyOptions));

        // Unlike the Guid, you CAN order by CombGuid - https://martendb.io/documents/identity.html#guid-identifiers
        // In this example there is no meaning for that...
        options.Schema.For<CounterState>().IdStrategy(new Marten.Schema.Identity.CombGuidIdGeneration());
    }

    private static readonly RetryStrategyOptions _retryStrategyOptions = new RetryStrategyOptions
    {
        ShouldHandle     = new PredicateBuilder().Handle<RankException>(),
        BackoffType      = DelayBackoffType.Exponential,
        MaxRetryAttempts = 3,
        Delay            = 150.Milliseconds(),
    };
}
