using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Infrastructure.Services;
using Testcontainers.MsSql;

namespace RateMyPet.Api;

public class ApiFixture : AppFixture<Program>
{
    private readonly MsSqlContainer databaseContainer = new MsSqlBuilder()
        .Build();

    protected override void ConfigureApp(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseSetting("ConnectionStrings:Database", databaseContainer.GetConnectionString());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
    }

    protected override async ValueTask PreSetupAsync()
    {
        await databaseContainer.StartAsync();
    }

    protected override async ValueTask SetupAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
