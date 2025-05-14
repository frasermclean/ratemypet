using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Core;
using RateMyPet.Database;
using RateMyPet.ImageHosting;
using RateMyPet.Infrastructure.Services;
using RateMyPet.Storage;

namespace RateMyPet.Api.Startup;

public static class ServiceRegistration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.AddDatabaseServices()
            .AddStorageServices();

        builder.Services
            .AddIdentity()
            .AddFastEndpoints()
            .AddInfrastructureServices(builder.Configuration)
            .AddImageHosting();

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
            // add CORS policy
            builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => policyBuilder
                .WithOrigins("http://localhost:4200")
                .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                .AllowAnyHeader()
                .AllowCredentials()
                .WithExposedHeaders("Location")
                .SetPreflightMaxAge(TimeSpan.FromMinutes(10)))
            );
        }

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
}
