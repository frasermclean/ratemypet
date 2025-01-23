using System.Diagnostics;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using RateMyPet.Infrastructure;

namespace RateMyPet.Api.Startup;

public static class AppConfiguration
{
    public static WebApplicationBuilder AddAzureAppConfiguration(this WebApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["APP_CONFIG_ENDPOINT"];

        // early exit if no endpoint is provided
        if (string.IsNullOrEmpty(endpoint))
        {
            Console.WriteLine("Azure App Configuration endpoint not found. Using local configuration only.");
            return builder;
        }

        var environment = builder.Environment.EnvironmentName;
        Console.WriteLine($"Connecting to Azure App Configuration at: {endpoint}, environment: {environment}");

        var startingCount = builder.Configuration.AsEnumerable().Count();
        var credential = TokenCredentialFactory.Create();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            builder.Configuration.AddAzureAppConfiguration(options => options.Connect(new Uri(endpoint), credential)
                .Select(KeyFilter.Any)
                .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
                .ConfigureKeyVault(keyVaultOptions => keyVaultOptions.SetCredential(credential))
            );

            var count = builder.Configuration.AsEnumerable().Count() - startingCount;
            var elapsed = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Azure App Configuration loaded {count} values in {elapsed}ms.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to Azure App Configuration: {ex.Message}");
        }

        return builder;
    }
}
