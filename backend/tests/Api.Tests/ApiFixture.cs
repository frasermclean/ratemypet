using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Initializer;

namespace RateMyPet.Api;

public class ApiFixture : AppFixture<Program>
{
    private readonly DatabaseProvider databaseProvider = new();
    private readonly StorageProvider storageProvider = new();

    public HttpClient AdministratorClient => CreateClient(httpClient =>
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue($"{TestAuthHandler.SchemeName}-{Role.Administrator}"));

    public HttpClient ContributorClient => CreateClient(httpClient =>
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue($"{TestAuthHandler.SchemeName}-{Role.Contributor}"));

    protected override void ConfigureApp(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseSetting("ConnectionStrings:database", databaseProvider.ConnectionString);
        hostBuilder.UseSetting("ConnectionStrings:blobs", storageProvider.ConnectionString);
        hostBuilder.UseSetting("ConnectionStrings:queues", storageProvider.ConnectionString);

        hostBuilder.ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders());
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
        // add initializer services
        services.AddScoped<DatabaseInitializer>()
            .AddScoped<StorageInitializer>();

        // add test auth handler scheme
        services.AddAuthentication(TestAuthHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
    }

    protected override async ValueTask PreSetupAsync()
    {
        await Task.WhenAll(
            databaseProvider.InitializeAsync(),
            storageProvider.InitializeAsync()
        );
    }

    protected override async ValueTask SetupAsync()
    {
        await using var scope = Services.CreateAsyncScope();

        // initialize database and storage
        await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().InitializeAsync();
        await scope.ServiceProvider.GetRequiredService<StorageInitializer>().InitializeAsync();
    }
}
