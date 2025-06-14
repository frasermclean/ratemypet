using EntityFramework.Exceptions.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RateMyPet.Database.Interceptors;

namespace RateMyPet.Database;

public static class ServiceRegistration
{
    public static TBuilder AddDatabaseServices<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddScoped<UserInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((provider, optionsBuilder) =>
        {
            optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Database"))
                .AddInterceptors(provider.GetRequiredService<UserInterceptor>())
                .UseExceptionProcessor();
        });

        builder.EnrichSqlServerDbContext<ApplicationDbContext>();

        return builder;
    }
}
