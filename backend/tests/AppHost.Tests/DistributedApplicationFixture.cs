using Aspire.Hosting;
using Aspire.Hosting.Testing;
using RateMyPet.AppHost.Tests;

[assembly: AssemblyFixture(typeof(DistributedApplicationFixture))]

namespace RateMyPet.AppHost.Tests;

public sealed class DistributedApplicationFixture : IAsyncLifetime
{
    private DistributedApplication? app;

    public async ValueTask InitializeAsync()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.AppHost>(cancellationToken);

        app = await appHost.BuildAsync(cancellationToken);

        await app.StartAsync(cancellationToken).WaitAsync(TimeSpan.FromMinutes(5), cancellationToken);
    }

    public HttpClient CreateHttpClient(string resourceName)
    {
        return app is not null
            ? app.CreateHttpClient(resourceName)
            : throw new InvalidOperationException("DistributedApplicationFixture has not been initialized.");
    }

    public ValueTask DisposeAsync() => app?.DisposeAsync() ?? ValueTask.CompletedTask;
}
