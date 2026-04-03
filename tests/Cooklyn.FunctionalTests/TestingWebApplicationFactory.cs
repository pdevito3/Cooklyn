namespace Cooklyn.FunctionalTests;

using Cooklyn.Server.Databases;
using Cooklyn.SharedTestHelpers.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

[CollectionDefinition(nameof(TestBase))]
public class TestingWebApplicationFactoryCollection : ICollectionFixture<TestingWebApplicationFactory> { }

public class TestingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer _dbContainer = null!;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(TestingConsts.FunctionalTestingEnvName);

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureServices(services =>
        {
            // Add migration hosted service
            services.AddHostedService<MigrationHostedService<AppDbContext>>();
        });
    }

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .Build();
        await _dbContainer.StartAsync();

        Environment.SetEnvironmentVariable(
            "ConnectionStrings__CooklynDb",
            _dbContainer.GetConnectionString());

        // Disable Aspire health checks that require database connectivity during startup
        Environment.SetEnvironmentVariable("Aspire__Npgsql__EntityFrameworkCore__PostgreSQL__DisableHealthChecks", "true");
    }

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}
