using RateMyPet.Database;
using RateMyPet.ServiceDefaults;
using RateMyPet.Storage;

namespace RateMyPet.Initializer;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults()
            .AddStorageServices()
            .AddDatabaseServices(DatabaseInitializer.SeedAsync);

        builder.Services.AddHostedService<WorkerService>()
            .AddScoped<DatabaseInitializer>()
            .AddScoped<StorageInitializer>();

        var app = builder.Build();

        app.Run();
    }
}
