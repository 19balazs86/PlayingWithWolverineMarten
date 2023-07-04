using Lamar;
using Marten;
using Marten.Services.Json;
using Oakton;
using Oakton.Resources;
using Weasel.Core;
using Wolverine;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Marten;
using WolverineHttpWithMarten.Endpoints;
using WolverineHttpWithMarten.Entities;
using WolverineHttpWithMarten.Infrastructure;
using WolverineHttpWithMarten.Middlewares;

namespace WolverineHttpWithMarten;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;
        IConfiguration configuration  = builder.Configuration;
        IHostBuilder hostBuilder      = builder.Host;

        bool isDevelopment = builder.Environment.IsDevelopment();

        // Add services to the container
        {
            hostBuilder.ApplyOaktonExtensions(); // app.RunOaktonCommands need this

            hostBuilder.UseWolverine(configureWolverine);

            services
                .AddMarten(options => configureMarten(options, configuration))
                .UseLightweightSessions()
                .IntegrateWithWolverine()
                .InitializeWith() // https://martendb.io/configuration/hostbuilder.html#eager-initialization-of-the-documentstore
                .ApplyWhen(isDevelopment, martenConf => martenConf.InitializeWith<InitialProductData>());

            // services.InitializeMartenWith<InitialProductData>(); // This also works

            services.AddProblemDetails();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

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
                // A local variable or function named 'result' is already defined in this scope
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

        // options.CodeGeneration.TypeLoadMode = TypeLoadMode.Static; // https://wolverine.netlify.app/guide/codegen.html#optimized-workflow

        configureLamarServices(options.Services);
    }

    private static void configureMarten(StoreOptions options, IConfiguration configuration)
    {
        string postgreSqlConnString = configuration.GetConnectionString("PostgreSQL")
            ?? throw new NullReferenceException("Missing PostgreSQL connection string");

        options.Connection(postgreSqlConnString);

        options.DatabaseSchemaName = "public"; // It is 'public' by default

        options.AutoCreateSchemaObjects = AutoCreate.All; // It is 'All' by default

        options.UseDefaultSerialization(serializerType: SerializerType.SystemTextJson, enumStorage: EnumStorage.AsString);

        // options.RegisterDocumentType<Product>(); // This is optional. The first time you add the document, it automatically creates the table.
    }

    private static void configureLamarServices(ServiceRegistry services)
    {
        services.AddResourceSetupOnStartup();
    }
}
