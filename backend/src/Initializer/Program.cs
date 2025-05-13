using RateMyPet.Database;
using RateMyPet.ServiceDefaults;

namespace RateMyPet.Initializer;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args)
            .AddServiceDefaults();

        builder.AddSqlServerDbContext<ApplicationDbContext>("database");

        builder.Services.AddHostedService<WorkerService>();

        var app = builder.Build();

        app.Run();
    }
}
