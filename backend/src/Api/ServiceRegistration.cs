using FastEndpoints;

namespace RateMyPet.Api;

public static class ServiceRegistration
{
    public static WebApplicationBuilder RegisterServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddFastEndpoints();

        return builder;
    }
}