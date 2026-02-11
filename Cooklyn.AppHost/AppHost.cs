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

    var minio = builder.AddContainer("minio", "quay.io/minio/minio")
        .WithHttpEndpoint(port: 9000, targetPort: 9000, name: "http")
        .WithHttpEndpoint(port: 9001, targetPort: 9001, name: "console")
        .WithEnvironment("MINIO_ROOT_USER", "minioadmin")
        .WithEnvironment("MINIO_ROOT_PASSWORD", "miniosecretkey")
        .WithArgs("server", "--console-address", ":9001", "/data")
        .WithVolume("cooklyn-minio", "/data");

    var server = builder.AddProject<Projects.Cooklyn_Server>("server")
        .WithReference(appDb)
        .WaitFor(appDb)
        .WaitFor(minio)
        .WithServerAuth(authProvider)
        .WithEnvironment(context =>
        {
            context.EnvironmentVariables["AWS__ServiceURL"] = minio.GetEndpoint("http");
        })
        .WithEnvironment("AWS__AccessKey", "minioadmin")
        .WithEnvironment("AWS__SecretKey", "miniosecretkey")
        .WithEnvironment("AWS__RecipeImagesBucket", "recipe-images")
        .WithHttpHealthCheck("/health", endpointName: "http")
        .WithExternalHttpEndpoints();
    postgres.WithParentRelationship(server);
    minio.WithParentRelationship(server);

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
        .WithEndpoint("http", e =>
        {
            e.Port = 5234;
            e.IsProxied = false;
        })
        .WithHttpHealthCheck("/health", endpointName: "http")
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
