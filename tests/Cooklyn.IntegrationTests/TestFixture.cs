namespace Cooklyn.IntegrationTests;

using Cooklyn.Server.Databases;
using Cooklyn.Server.Resources.Extensions;
using Cooklyn.SharedTestHelpers.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

[CollectionDefinition(nameof(TestFixture))]
public class TestFixtureCollection : ICollectionFixture<TestFixture> { }

public class TestFixture : IAsyncLifetime
{
    public static IServiceScopeFactory BaseScopeFactory = null!;
    private PostgreSqlContainer _dbContainer = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = TestingConsts.IntegrationTestingEnvName
        });

        // Start PostgreSQL container
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .Build();
        await _dbContainer.StartAsync();

        builder.Configuration[$"ConnectionStrings:{DatabaseConsts.DatabaseName}"] = _dbContainer.GetConnectionString();

        // Configure services
        builder.ConfigureServices();

        var services = builder.Services;

        var provider = services.BuildServiceProvider();
        BaseScopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

        // Run migrations
        await using var scope = BaseScopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
}
