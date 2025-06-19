using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateMyPet.Database.Interceptors;

namespace RateMyPet.Database;

public static class ServiceRegistration
{
    public static TBuilder AddDatabaseServices<TBuilder>(this TBuilder builder,
        Func<DbContext, bool, CancellationToken, Task>? seedAsync = null)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddScoped<UserInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((provider, optionsBuilder) =>
        {
            optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
                .AddInterceptors(provider.GetRequiredService<UserInterceptor>())
                .UseExceptionProcessor();

            if (seedAsync is not null)
            {
                optionsBuilder.UseAsyncSeeding(seedAsync);
            }
        });

        builder.EnrichSqlServerDbContext<ApplicationDbContext>();

        return builder;
    }
}
