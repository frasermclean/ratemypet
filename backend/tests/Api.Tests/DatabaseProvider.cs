using Testcontainers.MsSql;

namespace RateMyPet.Api.Tests;

public sealed class DatabaseProvider
{
    private readonly MsSqlContainer container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString => container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await container.StartAsync();
    }
}
