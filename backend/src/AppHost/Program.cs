namespace RateMyPet.AppHost;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var api = builder.AddProject<Projects.Api>("api");

        builder.Build().Run();
    }
}
