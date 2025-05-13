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

        var api = builder.AddProject<Projects.Api>("api")
            .WithReference(database)
            .WaitFor(database);

        builder.Build().Run();
    }
}
