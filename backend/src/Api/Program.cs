using RateMyPet.Api.Startup;

namespace RateMyPet.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var app = WebApplication.CreateBuilder(args)
            .AddAzureAppConfiguration()
            .RegisterServices()
            .Build()
            .ConfigureMiddleware();

        app.Run();
    }
}
