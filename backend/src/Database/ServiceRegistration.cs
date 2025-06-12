using EntityFramework.Exceptions.SqlServer;
using Microsoft.Extensions.Hosting;
using RateMyPet.Database.Interceptors;

namespace RateMyPet.Database;

public static class ServiceRegistration
{
    public static TBuilder AddDatabaseServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.AddSqlServerDbContext<ApplicationDbContext>("Database",
            configureDbContextOptions: optionsBuilder =>
            {
                optionsBuilder.AddInterceptors(new UserInterceptor())
                    .UseExceptionProcessor();
            });

        return builder;
    }
}
