using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateMyPet.Database;
using RateMyPet.ImageHosting;
using RateMyPet.Infrastructure.Services;
using RateMyPet.ServiceDefaults;
using RateMyPet.Storage;

namespace RateMyPet.Jobs;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = FunctionsApplication.CreateBuilder(args)
            .AddServiceDefaults()
            .ConfigureFunctionsWebApplication()
            .AddDatabaseServices()
            .AddStorageServices()
            .RegisterServices()
            .Build();

        host.Run();
    }

    private static FunctionsApplicationBuilder RegisterServices(this FunctionsApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .UseFunctionsWorkerDefaults();

        builder.Services.AddInfrastructureServices(builder.Configuration)
            .AddImageHosting();

        return builder;
    }
}
