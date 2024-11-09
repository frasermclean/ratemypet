using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using RateMyPet.Persistence;
using RateMyPet.Persistence.Models;

namespace RateMyPet.Api;

public static class ServiceRegistration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddPersistence()
            .AddIdentity()
            .AddFastEndpoints();

        return builder;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services.AddAuthorization();

        services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        return services;
    }
}