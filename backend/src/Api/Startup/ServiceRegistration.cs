using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Identity;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Azure;
using RateMyPet.Api.Options;
using RateMyPet.Api.Services;
using RateMyPet.Persistence.Models;
using RateMyPet.Persistence.Services;

namespace RateMyPet.Api.Startup;

public static class ServiceRegistration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddApiServices()
            .AddPersistence(builder.Configuration)
            .AddIdentity()
            .AddFastEndpoints();

        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
            };
        });

        builder.Services.AddAzureClients(factoryBuilder =>
        {
            var connectionString = builder.Configuration.GetConnectionString("Storage");
            factoryBuilder.AddBlobServiceClient(connectionString);

            var emailClientEndpoint = new Uri(builder.Configuration["EmailSender:Endpoint"]!);
            factoryBuilder.AddEmailClient(emailClientEndpoint);

            factoryBuilder.UseCredential(new DefaultAzureCredential());
        });

        // json serialization options
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });

        // development services
        if (builder.Environment.IsDevelopment())
        {
            // add cors policy
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policyBuilder => policyBuilder
                    .WithOrigins(builder.Configuration["Frontend:BaseUrl"]!)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Location")
                );
            });
        }

        return builder;
    }

    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<ImageProcessor>();

        services.AddOptions<ImageProcessorOptions>()
            .BindConfiguration(ImageProcessorOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddOptions<EmailSenderOptions>()
            .BindConfiguration(EmailSenderOptions.SectionName)
            .ValidateDataAnnotations();

        services.AddOptions<FrontendOptions>()
            .BindConfiguration(FrontendOptions.SectionName)
            .ValidateDataAnnotations();

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
}
