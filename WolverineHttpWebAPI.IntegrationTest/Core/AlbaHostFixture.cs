using Alba;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oakton;
using Testcontainers.PostgreSql;
using Wolverine;

namespace WolverineHttpWebAPI.IntegrationTest.Core;

public sealed class AlbaHostFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("database")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public IAlbaHost AlbaWebHost { get; set; } = default!;

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        // Workaund for Oakton with WebApplicationBuilder
        OaktonEnvironment.AutoStartHost = true;

        AlbaWebHost = await AlbaHost.For<Program>(configureWebHostBuilder);
    }

    private void configureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
        //webHostBuilder.ConfigureAppConfiguration(configureAppConfiguration);

        webHostBuilder.UseConfiguration(createAppConfiguration());

        webHostBuilder.ConfigureServices(configureServices);
    }

    private void configureServices(IServiceCollection services)
    {
        services.DisableAllExternalWolverineTransports();

        services.InitializeMartenWith<InitialProductData>();
    }

    // This is working in PlayingWithTestContainers. But now, somehow the in-memory configuration applied later than you GetConnectionString
    //private void configureAppConfiguration(IConfigurationBuilder configurationBuilder)
    //{
    //    var configurationOverridden = new Dictionary<string, string>
    //    {
    //        ["ConnectionStrings:PostgreSQL"] = _postgreSqlContainer.GetConnectionString()
    //    };

    //    configurationBuilder.AddInMemoryCollection(configurationOverridden!);
    //}

    private IConfiguration createAppConfiguration()
    {
        var builder = new ConfigurationBuilder();

        var configurationOverridden = new Dictionary<string, string>
        {
            ["ConnectionStrings:PostgreSQL"] = _postgreSqlContainer.GetConnectionString()
        };

        builder.AddInMemoryCollection(configurationOverridden!);

        return builder.Build();
    }

    public async Task DisposeAsync()
    {
        await AlbaWebHost.DisposeAsync();

        await _postgreSqlContainer.StopAsync();
    }
}
