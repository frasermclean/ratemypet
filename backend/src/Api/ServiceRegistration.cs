using FastEndpoints;
using RateMyPet.Persistence;

namespace RateMyPet.Api;

public static class ServiceRegistration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddPersistence()
            .AddFastEndpoints();

        return builder;
    }
}