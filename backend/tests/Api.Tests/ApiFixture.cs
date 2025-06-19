using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RateMyPet.Core;
using RateMyPet.Database;
using RateMyPet.Initializer;

namespace RateMyPet.Api.Tests;

public class ApiFixture : AppFixture<Program>
{
    private readonly DatabaseProvider databaseProvider = new();
    private readonly StorageProvider storageProvider = new();

    public const string AdministratorUserName = "test-admin";
    public const string AdministratorEmail = "test-admin@ratemy.pet";
    public const string AdministratorPassword = "TestPassword123!";

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
        services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(databaseProvider.ConnectionString)
                .UseAsyncSeeding(DatabaseInitializer.SeedAsync);
        });

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

        await CreateUsersAsync(scope.ServiceProvider);
    }

    private static async Task CreateUsersAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        if (await userManager.FindByNameAsync(AdministratorUserName) is null)
        {
            // create administrator user
            var user = new User
            {
                UserName = AdministratorUserName,
                Email = AdministratorEmail,
                EmailConfirmed = true,
            };
            await userManager.CreateAsync(user, AdministratorPassword);
            await userManager.AddToRolesAsync(user, [Role.Contributor, Role.Administrator]);
        }
    }
}
