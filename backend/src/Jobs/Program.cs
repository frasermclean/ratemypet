using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.OpenTelemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateMyPet.AiServices;
using RateMyPet.Database;
using RateMyPet.Email;
using RateMyPet.ImageHosting;
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

        builder.Services.AddImageHosting()
            .AddEmailSending()
            .AddAiServices();

        return builder;
    }
}
