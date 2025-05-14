using Microsoft.Extensions.Hosting;

namespace RateMyPet.Database;

public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddDatabaseServices(this IHostApplicationBuilder builder)
    {
        builder.AddSqlServerDbContext<ApplicationDbContext>("Database");

        return builder;
    }
}
