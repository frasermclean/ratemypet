using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateMyPet.Database;
using RateMyPet.Infrastructure.Services;
using RateMyPet.Storage;

namespace RateMyPet.Jobs;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = FunctionsApplication.CreateBuilder(args)
            .RegisterServices()
            .Build();

        host.Run();
    }

    private static FunctionsApplicationBuilder RegisterServices(this FunctionsApplicationBuilder builder)
    {
        builder.AddDatabaseServices()
            .AddStorageServices();

        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddInfrastructureServices(builder.Configuration);

        return builder;
    }
}
