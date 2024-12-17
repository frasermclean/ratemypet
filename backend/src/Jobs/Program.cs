using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using RateMyPet.Jobs.Startup;

namespace RateMyPet.Jobs;

public static class Program
{
    public static void Main(string[] args)
    {
        var host = FunctionsApplication.CreateBuilder(args)
            .AddAzureAppConfiguration()
            .RegisterServices()
            .Build();

        host.Run();

    }
}
