using RateMyPet.Database;
using RateMyPet.ServiceDefaults;

namespace RateMyPet.Initializer;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults()
            .AddDatabaseServices();

        builder.Services.AddHostedService<WorkerService>()
            .AddScoped<DatabaseInitializer>();

        var app = builder.Build();

        app.Run();
    }
}
