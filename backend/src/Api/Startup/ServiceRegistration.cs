using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
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

        builder.Services.AddOptions<EmailSenderOptions>()
            .BindConfiguration(EmailSenderOptions.SectionName)
            .ValidateDataAnnotations();

        builder.Services.AddOptions<FrontendOptions>()
            .BindConfiguration(FrontendOptions.SectionName)
            .ValidateDataAnnotations();

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
            var connectionString = configuration.GetConnectionString("Storage");
            factoryBuilder.AddBlobServiceClient(connectionString);

            var emailClientEndpoint = new Uri(configuration["EmailSender:Endpoint"]!);
            factoryBuilder.AddEmailClient(emailClientEndpoint);

            factoryBuilder.UseCredential(TokenCredentialFactory.Create());
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
            .AddApiEndpoints()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddTransient<IEmailSender, IdentityEmailSender>();

        services.AddHostedService<SecurityInitializer>();

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
