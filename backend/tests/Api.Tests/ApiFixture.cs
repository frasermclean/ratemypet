using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api;

public class ApiFixture : AppFixture<Program>
{
    private readonly DatabaseProvider databaseProvider = new();

    protected override void ConfigureApp(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseSetting("ConnectionStrings:Database", databaseProvider.ConnectionString);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
    }

    protected override async ValueTask PreSetupAsync()
    {
        await databaseProvider.InitializeAsync();
    }

    protected override async ValueTask SetupAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }
}
