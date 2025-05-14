namespace RateMyPet.AppHost;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var saPassword = builder.AddParameter("SaPassword", true);

        var sqlServer = builder.AddSqlServer("sql-server", saPassword)
            .WithDataVolume("rmp-sql-server");

        var database = sqlServer.AddDatabase("database", "RateMyPet");

        var storage = builder.AddAzureStorage("storage")
            .RunAsEmulator(resourceBuilder => resourceBuilder.WithDataVolume("rmp-storage"));

        var blobStorage = storage.AddBlobs("blobs");
        var queueStorage = storage.AddQueues("queues");

        var initializer = builder.AddProject<Projects.Initializer>("initializer")
            .WithReference(database)
            .WithReference(blobStorage)
            .WithReference(queueStorage)
            .WaitFor(database)
            .WaitFor(blobStorage)
            .WaitFor(queueStorage);

        var api = builder.AddProject<Projects.Api>("api")
            .WithReference(database)
            .WithReference(blobStorage)
            .WithReference(queueStorage)
            .WaitForCompletion(initializer)
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}
