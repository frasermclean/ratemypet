using Aspire.Hosting.Azure;

namespace RateMyPet.AppHost;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args)
            .AddSqlServerWithDatabase(out var database)
            .AddAzureStorage(out var storage, out var blobs, out var queues);

        var initializer = builder.AddProject<Projects.Initializer>("initializer")
            .WithReference(database)
            .WithReference(blobs)
            .WithReference(queues)
            .WaitFor(database)
            .WaitFor(blobs)
            .WaitFor(queues);

        var api = builder.AddProject<Projects.Api>("api")
            .WaitForCompletion(initializer)
            .WithReference(database)
            .WithReference(blobs)
            .WithReference(queues)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health");

        builder.AddAzureFunctionsProject<Projects.Jobs>("jobs")
            .WaitForCompletion(initializer)
            .WithHostStorage(storage)
            .WithReference(database)
            .WithReference(blobs)
            .WithExternalHttpEndpoints();

        builder.AddNpmApp("frontend", "../../../frontend")
            .WaitFor(api)
            .WithReference(api)
            .WithHttpEndpoint(4200, env: "PORT")
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }

    private static IDistributedApplicationBuilder AddSqlServerWithDatabase(
        this IDistributedApplicationBuilder builder,
        out IResourceBuilder<SqlServerDatabaseResource> database)
    {
        var saPassword = builder.AddParameter("SaPassword", true);

        var sqlServer = builder.AddSqlServer("sql-server", saPassword)
            .WithContainerName("ratemypet-sql-server")
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume("ratemypet-sql-server")
            .WithEndpoint(port: 1433, targetPort: 1433, name: "direct", isProxied: false);

        database = sqlServer.AddDatabase("database", "RateMyPet");

        return builder;
    }

    private static IDistributedApplicationBuilder AddAzureStorage(
        this IDistributedApplicationBuilder builder,
        out IResourceBuilder<AzureStorageResource> storage,
        out IResourceBuilder<AzureBlobStorageResource> blobs,
        out IResourceBuilder<AzureQueueStorageResource> queues)
    {
        storage = builder.AddAzureStorage("storage")
            .RunAsEmulator(resourceBuilder => resourceBuilder
                .WithContainerName("ratemypet-storage")
                .WithLifetime(ContainerLifetime.Persistent)
                .WithDataVolume("ratemypet-storage"));

        blobs = storage.AddBlobs("blobs");
        queues = storage.AddQueues("queues");

        return builder;
    }
}
