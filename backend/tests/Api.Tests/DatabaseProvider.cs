using Testcontainers.MsSql;

namespace RateMyPet.Api;

public sealed class DatabaseProvider : IAsyncDisposable
{
    private readonly MsSqlContainer container = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public string ConnectionString => container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await container.DisposeAsync();
    }
}
