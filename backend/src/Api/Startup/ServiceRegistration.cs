using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Azure.Storage.Blobs;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using OpenTelemetry.Resources;
using RateMyPet.Api.Options;
using RateMyPet.Api.Services;
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
            .AddSingleton<ImageProcessor>();

        builder.Services.AddOptions<ImageProcessorOptions>()
            .BindConfiguration(ImageProcessorOptions.SectionName)
            .ValidateDataAnnotations();

        builder.Services.AddOptions<EmailOptions>()
            .BindConfiguration(EmailOptions.SectionName)
            .ValidateDataAnnotations();

        builder.Services.AddOptions<FrontendOptions>()
            .BindConfiguration(FrontendOptions.SectionName)
            .ValidateDataAnnotations();

        // open telemetry
        builder.Services.AddOpenTelemetry()
            .UseAzureMonitor(options => options.Credential = TokenCredentialFactory.Create())
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder.AddAttributes(new Dictionary<string, object>
                {
                    { "service.name", "api" },
                    { "service.namespace", "backend" }
                });
            });

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

        services.AddKeyedScoped<IBlobContainerManager>(BlobContainerNames.OriginalImages, (provider, _) =>
        {
            var containerClient = provider.GetRequiredService<BlobServiceClient>()
                .GetBlobContainerClient(BlobContainerNames.OriginalImages);
            return new BlobContainerManager(containerClient);
        });

        services.AddAzureClients(factoryBuilder =>
        {
            factoryBuilder.AddBlobServiceClient(new Uri(configuration["Storage:BlobEndpoint"]!));
            factoryBuilder.AddEmailClient(new Uri(configuration["Email:AcsEndpoint"]!));
            factoryBuilder.UseCredential(TokenCredentialFactory.Create());
            factoryBuilder.ConfigureDefaults(options => options.Diagnostics.IsLoggingEnabled = false);
        });

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddAuthentication(IdentityConstants.BearerScheme)
            .AddBearerToken(IdentityConstants.BearerScheme);

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

        services.AddTransient<IEmailSender, EmailSender>();

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
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Location")
            );
        });

        return services;
    }
}
