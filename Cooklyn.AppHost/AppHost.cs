using Cooklyn.AppHost;
using Serilog;
using Serilog.Events;

// Bootstrap logger to catch startup errors before host is built
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Aspire.Hosting.Dcp", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Cooklyn.AppHost");

    var builder = DistributedApplication.CreateBuilder(args);

    builder.Services.AddSerilog((_, lc) => lc
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithProperty("Application", "Cooklyn.AppHost"));

    var authProvider = AuthProviders.Keycloak(builder);

    var postgres = builder.AddPostgres("postgres")
        .WithDataVolume("cooklyn-postgres");
    var appDb = postgres.AddDatabase("appdb");

    var server = builder.AddProject<Projects.Cooklyn_Server>("server")
        .WithReference(appDb)
        .WaitFor(appDb)
        .WithServerAuth(authProvider)
        .WithHttpHealthCheck("/health")
        .WithExternalHttpEndpoints();
    postgres.WithParentRelationship(server);

    var webfrontend = builder.AddViteApp("webfrontend", "../frontend")
        .WithEndpoint("http", endpoint =>
        {
            endpoint.Port = 6179;
            endpoint.IsProxied = false;
        })
        .WithPnpm();

    var bff = builder.AddProject<Projects.Cooklyn_Bff>("bff")
        .WithBffAuth(authProvider)
        .WithReference(server)
        .WithReference(webfrontend)
        .WaitFor(server)
        .WithHttpHealthCheck("/health")
        .WithExternalHttpEndpoints()
        .WithParentRelationship(webfrontend);

    if (authProvider.AuthResource is not null)
    {
        server.WaitFor(authProvider.AuthResource);
        bff.WaitFor(authProvider.AuthResource);
    }

    webfrontend
        .WithReference(bff)
        .WaitFor(bff);

    server.PublishWithContainerFiles(webfrontend, "wwwroot");

    builder.Build().Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "AppHost terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
