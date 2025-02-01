﻿using Azure.Storage.Queues;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RateMyPet.Infrastructure;
using RateMyPet.Infrastructure.Services;

namespace RateMyPet.Api;

public class ApiFixture : AppFixture<Program>
{
    private readonly DatabaseProvider databaseProvider = new();
    private readonly StorageProvider storageProvider = new();

    protected override void ConfigureApp(IWebHostBuilder hostBuilder)
    {
        hostBuilder.UseSetting("ConnectionStrings:Database", databaseProvider.ConnectionString);
        hostBuilder.UseSetting("ConnectionStrings:Storage", storageProvider.ConnectionString);
    }

    protected override void ConfigureServices(IServiceCollection services)
    {
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

        // create database
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        // create storage queues
        var queueServiceClient = scope.ServiceProvider.GetRequiredService<QueueServiceClient>();
        await queueServiceClient.CreateQueueAsync(QueueNames.ForgotPassword);
        await queueServiceClient.CreateQueueAsync(QueueNames.PostAdded);
        await queueServiceClient.CreateQueueAsync(QueueNames.RegisterConfirmation);
    }
}
