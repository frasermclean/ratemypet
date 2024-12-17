using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using RateMyPet.Persistence;

namespace RateMyPet.Jobs.Startup;

public static class AppConfiguration
{
    public static FunctionsApplicationBuilder AddAzureAppConfiguration(this FunctionsApplicationBuilder builder)
    {
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var endpoint = builder.Configuration["APP_CONFIG_ENDPOINT"];
            if (string.IsNullOrEmpty(endpoint))
            {
                return;
            }

            var credential = TokenCredentialFactory.Create();
            options.Connect(new Uri(endpoint), credential)
                .Select(KeyFilter.Any)
                .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
                .ConfigureKeyVault(keyVaultOptions => keyVaultOptions.SetCredential(credential));
        });

        return builder;
    }
}
