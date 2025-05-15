using RateMyPet.Api.Startup;
using RateMyPet.ServiceDefaults;

namespace RateMyPet.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var app = WebApplication.CreateBuilder(args)
            .AddServiceDefaults()
            .AddAzureAppConfiguration()
            .RegisterServices()
            .Build()
            .ConfigureMiddleware();

        app.Run();
    }
}
