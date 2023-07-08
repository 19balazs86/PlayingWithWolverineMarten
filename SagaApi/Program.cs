using Lamar;
using Marten;
using Marten.Services.Json;
using Oakton;
using Oakton.Resources;
using SagaApi.Sagas;
using Wolverine;
using Wolverine.Marten;

namespace SagaApi;

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
            app.MapGet("/registration/{email}", Endpoints.EmailConfirmation_Start);
            app.MapGet("/confirm/{sagaId}",     Endpoints.EmailConfirmation_Confirm);
        }

        writeConsoleLog(app.Logger);

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

        options.UseDefaultSerialization(serializerType: SerializerType.SystemTextJson);

        // User can not initiate registration with the same email
        options.Schema.For<EmailConfirmationSaga>().UniqueIndex(x => x.Email);
    }

    private static void configureLamarServices(this ServiceRegistry services)
    {
        services.AddResourceSetupOnStartup();
    }

    private static void writeConsoleLog(ILogger logger)
    {
        new TaskFactory().StartNew(async () =>
        {
            await Task.Delay(1_500);

            logger.LogWarning("Open the URL -> http://localhost:5076/registration/balazs@domain.com");
        });
    }
}
