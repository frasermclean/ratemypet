﻿using Microsoft.Extensions.Configuration.AzureAppConfiguration;

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

        Console.WriteLine("Connecting to Azure App Configuration at: " + endpoint);
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            var credential = TokenCredentialFactory.Create();
            options.Connect(new Uri(endpoint), credential)
                .Select(KeyFilter.Any)
                .Select(KeyFilter.Any, builder.Environment.EnvironmentName)
                .ConfigureKeyVault(keyVaultOptions => keyVaultOptions.SetCredential(credential));
        });

        return builder;
    }
}