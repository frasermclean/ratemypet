using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RateMyPet.Persistence.Services;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddDbContextFactory<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("Database");
            optionsBuilder.UseSqlServer(connectionString);
        });

        return services;
    }
}
