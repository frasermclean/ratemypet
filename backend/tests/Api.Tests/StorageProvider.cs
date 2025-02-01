using Testcontainers.Azurite;

namespace RateMyPet.Api;

public sealed class StorageProvider
{
    private readonly AzuriteContainer storageContainer = new AzuriteBuilder()
        .WithImage("mcr.microsoft.com/azure-storage/azurite:latest")
        .Build();

    public string ConnectionString => storageContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await storageContainer.StartAsync();
    }
}
