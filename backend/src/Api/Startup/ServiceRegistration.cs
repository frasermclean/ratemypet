using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Core;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;
using SixLabors.ImageSharp.Web.Caching.Azure;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers.Azure;

namespace RateMyPet.Api.Startup;

public static class ServiceRegistration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity()
            .AddImageSharp(builder.Configuration)
            .AddFastEndpoints()
            .AddInfrastructureServices(builder.Configuration)
            .AddDevelopmentCors(builder.Configuration, builder.Environment);

        // open telemetry
        builder.Services.AddOpenTelemetry()
            .UseAzureMonitor(options => options.Credential = TokenCredentialFactory.Create());

        // problem details service
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
            };
        });

        // json serialization options
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });

        return builder;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddIdentityCookies();

        services.AddAuthorization();

        services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<Role>()
            .AddSignInManager()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "rmp_app";
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });

        return services;
    }

    /// <summary>
    /// Add ImageSharp.Web image processing middleware services
    /// </summary>
    private static IServiceCollection AddImageSharp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddImageSharp()
            .ClearProviders()
            .AddProvider<AzureBlobStorageImageProvider>()
            .Configure<AzureBlobStorageImageProviderOptions>(options => options.BlobContainers.Add(
                new AzureBlobContainerClientOptions
                {
                    ConnectionString = configuration.GetConnectionString("Storage"),
                    ContainerName = BlobContainerNames.OriginalImages
                }))
            .SetCache<AzureBlobStorageCache>()
            .Configure<AzureBlobStorageCacheOptions>(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("Storage")!;
                options.ContainerName = BlobContainerNames.PostImages;
            });

        return services;
    }

    /// <summary>
    /// Add CORS policy for local development environment
    /// </summary>
    private static void AddDevelopmentCors(this IServiceCollection services,
        ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        // only add CORS policy in development environment
        if (!environment.IsDevelopment())
        {
            return;
        }

        var frontEndBaseUrl = configuration["Frontend:BaseUrl"];
        services.AddCors(options => options.AddDefaultPolicy(policyBuilder => policyBuilder
            .WithOrigins(frontEndBaseUrl!)
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
            .AllowAnyHeader()
            .AllowCredentials()
            .WithExposedHeaders("Location")
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10))));
    }
}
