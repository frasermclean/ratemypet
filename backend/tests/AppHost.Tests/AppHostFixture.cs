using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.Hosting;
using RateMyPet.AppHost.Tests;

[assembly: AssemblyFixture(typeof(AppHostFixture))]

namespace RateMyPet.AppHost.Tests;

public sealed class AppHostFixture() : DistributedApplicationFactory(typeof(Projects.AppHost)), IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        await StartAsync();
    }
}
