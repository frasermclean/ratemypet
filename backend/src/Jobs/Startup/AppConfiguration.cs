using System.Diagnostics;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using RateMyPet.Persistence;

namespace RateMyPet.Jobs.Startup;

public static class AppConfiguration
{
    public static FunctionsApplicationBuilder AddAzureAppConfiguration(this FunctionsApplicationBuilder builder)
    {
        var endpoint = builder.Configuration["APP_CONFIG_ENDPOINT"];
        if (string.IsNullOrEmpty(endpoint))
        {
            Console.WriteLine("Azure App Configuration endpoint not found. Using local configuration only.");
            return builder;
        }

        var environment = builder.Configuration["APP_ENVIRONMENT"];
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
