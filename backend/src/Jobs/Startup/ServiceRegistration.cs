using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Jobs.Startup;

public static class ServiceRegistration
{
    public static FunctionsApplicationBuilder RegisterServices(this FunctionsApplicationBuilder builder)
    {
        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddInfrastructureServices(builder.Configuration);

        return builder;
    }
}
