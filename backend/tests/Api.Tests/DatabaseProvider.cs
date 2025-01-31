using Testcontainers.MsSql;

namespace RateMyPet.Api;

public sealed class DatabaseProvider : IAsyncDisposable
{
    private readonly MsSqlContainer container = new MsSqlBuilder()
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
