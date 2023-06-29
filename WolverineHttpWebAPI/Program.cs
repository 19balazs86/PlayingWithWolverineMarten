using Lamar;
using Marten;
using Oakton;
using Oakton.Resources;
using Weasel.Core;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Marten;
using WolverineHttpWebAPI.Endpoints;
using WolverineHttpWebAPI.Entities;
using WolverineHttpWebAPI.Infrastructure;
using WolverineHttpWebAPI.Middlewares;

namespace WolverineHttpWebAPI;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;
        IHostBuilder hostBuilder      = builder.Host;

        // Add services to the container
        {
            hostBuilder.ApplyOaktonExtensions(); // app.RunOaktonCommands need this

            hostBuilder.UseWolverine(configureWolverine);

            services.AddProblemDetails();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services
                .AddMarten(configureMarten)
                .UseLightweightSessions()
                .IntegrateWithWolverine();

            services.AddHostedService<DataBaseInitializer_HostedService>();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseStatusCodePages(); // Generates the problem details for 400-599 response

            app.MapWolverineEndpoints(options =>
            {
                // Right now this Middleware cause an exception in case of IResult response
                options.UseFluentValidationProblemDetailMiddleware();
            });

            app.MapFallback(HelloEndpoints.PageNotFound);
        }

        return await app.RunOaktonCommands(args);
    }

    private static void configureWolverine(HostBuilderContext context, WolverineOptions options)
    {
        // https://wolverine.netlify.app/guide/durability/marten/#transactional-middleware
        // This middleware will apply to the HTTP endpoints as well
        // options.Policies.AutoApplyTransactions(); // Or use [Transactional] attribute

        // Setting up the outbox on all locally handled background tasks
        options.Policies.UseDurableLocalQueues();

        options.Policies.ForMessagesOfType<IProductLookup>().AddMiddleware<ProductLookupMiddleware>();

        // options.LocalQueueFor<ProductCreated>().UseDurableInbox();

        options.UseFluentValidation();

        options.Services.configureLamarServices();
    }

    private static void configureMarten(StoreOptions options)
    {
        options.Connection("Host=localhost;Port=5432;Username=postgres;Password=postgrespw;Database=postgres;");

        options.DatabaseSchemaName = "public";

        options.AutoCreateSchemaObjects = AutoCreate.All; // It is 'All' by default

        options.UseDefaultSerialization(enumStorage: EnumStorage.AsString);
    }

    private static void configureLamarServices(this ServiceRegistry services)
    {
        services.AddResourceSetupOnStartup();
    }
}
