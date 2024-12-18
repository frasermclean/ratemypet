using System.Diagnostics;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using RateMyPet.Persistence;

namespace RateMyPet.Api.Startup;

public static class AppConfiguration
{
    public static WebApplicationBuilder AddAzureAppConfiguration(this WebApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["APP_CONFIG_ENDPOINT"];
        if (string.IsNullOrEmpty(endpoint))
        {
            Console.WriteLine("Azure App Configuration endpoint not found. Using local configuration only.");
            return builder;
        }

        var environment = builder.Environment.EnvironmentName;
        Console.WriteLine($"Connecting to Azure App Configuration at: {endpoint}, environment: {environment}");
        var stopwatch = Stopwatch.StartNew();
        var startingCount = builder.Configuration.AsEnumerable().Count();
        var credential = TokenCredentialFactory.Create();

        try
        {
            builder.Configuration.AddAzureAppConfiguration(options => options.Connect(new Uri(endpoint), credential)
                    .Select(KeyFilter.Any)
                    .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
                    .ConfigureKeyVault(keyVaultOptions => keyVaultOptions.SetCredential(credential))
            );

            var count = builder.Configuration.AsEnumerable().Count() - startingCount;
            var elapsed = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"Azure App Configuration loaded in {elapsed}ms. {count} settings loaded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to Azure App Configuration: {ex.Message}");
        }

        return builder;
    }
}
