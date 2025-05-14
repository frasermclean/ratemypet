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

        var api = builder.AddProject<Projects.Api>("api")
            .WithReference(database)
            .WithReference(blobStorage)
            .WithReference(queueStorage)
            .WaitFor(database)
            .WithExternalHttpEndpoints();

        var initializer = builder.AddProject<Projects.Initializer>("initializer")
            .WithReference(database)
            .WaitFor(database);

        builder.Build().Run();
    }
}
