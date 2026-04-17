namespace Cooklyn.Server.Resources.Extensions;

using Databases;
using Domain.Recipes.Importing;
using Domain.Recipes.Importing.CopyMeThat;
using Exceptions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;

public static class WebAppServiceConfiguration
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<DomainExceptionHandler>();
        builder.Services.AddFusionCache()
            .WithDefaultEntryOptions(options =>
            {
                options.Duration = TimeSpan.FromMinutes(5);
                options.IsFailSafeEnabled = true;
                options.FailSafeMaxDuration = TimeSpan.FromHours(2);
                options.FailSafeThrottleDuration = TimeSpan.FromSeconds(30);
            });
        builder.Services.AddApplicationServices();
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddApiVersioningExtension();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerExtension(builder.Configuration);

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var frontendUrl = builder.Configuration["services:webfrontend:https:0"]
                    ?? builder.Configuration["services:webfrontend:http:0"]
                    ?? "http://localhost:6179";
                policy.WithOrigins(frontendUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connectionString = builder.Configuration.GetConnectionString(DatabaseConsts.DatabaseName);

            options.UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        });
        builder.EnrichNpgsqlDbContext<AppDbContext>();

        builder.Services.AddS3FileStorage(builder.Configuration);

        // Recipe import
        builder.Services.AddSingleton<IRecipeSourceParser, JsonLdRecipeParser>();
        builder.Services.AddScoped<ICmtImportService, CmtImportService>();
        builder.Services.AddHttpClient("RecipeImport", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "Mozilla/5.0 (compatible; Cooklyn/1.0; +https://github.com/cooklyn)");
        });

        // Allow large file uploads for recipe import (100 MB)
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 104_857_600;
        });
    }
}
