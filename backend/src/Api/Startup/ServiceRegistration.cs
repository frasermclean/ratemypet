using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Storage.Blobs;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using RateMyPet.Core;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Startup;

public static class ServiceRegistration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddPersistence(builder.Configuration)
            .AddIdentity()
            .AddEnvironmentServices(builder.Environment, builder.Configuration)
            .AddFastEndpoints()
            .AddSingleton<IMessagePublisher, MessagePublisher>();

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

    private static IServiceCollection AddPersistence(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.AddDbContextFactory<ApplicationDbContext>(builder =>
        {
            var connectionString = configuration.GetConnectionString("Database");
            builder.UseSqlServer(connectionString);
        });

        services.AddAzureClients(factoryBuilder =>
        {
            // use connection string if endpoints are not provided
            var blobEndpoint = configuration["Storage:BlobEndpoint"];
            var queueEndpoint = configuration["Storage:QueueEndpoint"];
            if (string.IsNullOrEmpty(blobEndpoint) || string.IsNullOrEmpty(queueEndpoint))
            {
                var connectionString = configuration.GetConnectionString("Storage");
                factoryBuilder.AddBlobServiceClient(connectionString);
                factoryBuilder.AddQueueServiceClient(connectionString);
            }
            else
            {
                factoryBuilder.AddBlobServiceClient(new Uri(blobEndpoint));
                factoryBuilder.AddQueueServiceClient(new Uri(queueEndpoint));
            }

            factoryBuilder.UseCredential(TokenCredentialFactory.Create());
            factoryBuilder.ConfigureDefaults(options => options.Diagnostics.IsLoggingEnabled = false);
        });

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.OriginalImages, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.OriginalImages)));

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.PostImages, (provider, _) =>
            new BlobContainerManager(provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.PostImages)));

        return services;
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
            options.Cookie.Name = "RateMyPet.Auth";
            options.Cookie.SameSite = SameSiteMode.None;

            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });

        return services;
    }

    private static IServiceCollection AddEnvironmentServices(this IServiceCollection services,
        IWebHostEnvironment environment, ConfigurationManager configuration)
    {
        if (!environment.IsDevelopment())
        {
            return services;
        }

        // cors policy for frontend development
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder => policyBuilder
                .WithOrigins(configuration["Frontend:BaseUrl"]!)
                .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Location")
                .SetPreflightMaxAge(TimeSpan.FromMinutes(10))
            );
        });

        return services;
    }
}
