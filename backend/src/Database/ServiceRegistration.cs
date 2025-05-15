using Microsoft.Extensions.Hosting;

namespace RateMyPet.Database;

public static class ServiceRegistration
{
    public static TBuilder AddDatabaseServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddSqlServerDbContext<ApplicationDbContext>("Database");

        return builder;
    }
}
