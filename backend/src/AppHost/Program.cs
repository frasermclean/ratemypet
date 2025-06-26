using Aspire.Hosting.Azure;

namespace RateMyPet.AppHost;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args)
            .AddSqlServerWithDatabase(out var database)
            .AddAzureStorage(out var storage, out var blobs, out var queues)
            .AddAiServicesParameters(out var aiServicesComputerVisionEndpoint, out var aiServicesContentSafetyEndpoint)
            .AddCloudinaryParameters(out var cloudinaryCloudName, out var cloudinaryApiKey, out var cloudinaryApiSecret)
            .AddEmailParameters(out var emailAcsEndpoint, out var emailFrontendBaseUrl, out var emailSenderAddress);

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
            .WithEnvironment("Cloudinary__CloudName", cloudinaryCloudName)
            .WithEnvironment("Cloudinary__ApiKey", cloudinaryApiKey)
            .WithEnvironment("Cloudinary__ApiSecret", cloudinaryApiSecret)
            .WithExternalHttpEndpoints()
            .WithHttpHealthCheck("/health");

        builder.AddAzureFunctionsProject<Projects.Jobs>("jobs")
            .WaitForCompletion(initializer)
            .WithHostStorage(storage)
            .WithReference(database)
            .WithReference(blobs)
            .WithEnvironment("AiServices__ComputerVisionEndpoint", aiServicesComputerVisionEndpoint)
            .WithEnvironment("AiServices__ContentSafetyEndpoint", aiServicesContentSafetyEndpoint)
            .WithEnvironment("Cloudinary__CloudName", cloudinaryCloudName)
            .WithEnvironment("Cloudinary__ApiKey", cloudinaryApiKey)
            .WithEnvironment("Cloudinary__ApiSecret", cloudinaryApiSecret)
            .WithEnvironment("Email__AcsEndpoint", emailAcsEndpoint)
            .WithEnvironment("Email__FrontendBaseUrl", emailFrontendBaseUrl)
            .WithEnvironment("Email__SenderAddress", emailSenderAddress)
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

    private static IDistributedApplicationBuilder AddAiServicesParameters(
        this IDistributedApplicationBuilder builder,
        out IResourceBuilder<ParameterResource> aiServicesComputerVisionEndpoint,
        out IResourceBuilder<ParameterResource> aiServicesContentSafetyEndpoint)
    {
        aiServicesComputerVisionEndpoint = builder.AddParameter("aiServicesComputerVisionEndpoint");
        aiServicesContentSafetyEndpoint = builder.AddParameter("aiServicesContentSafetyEndpoint");

        return builder;
    }

    private static IDistributedApplicationBuilder AddCloudinaryParameters(
        this IDistributedApplicationBuilder builder,
        out IResourceBuilder<ParameterResource> cloudinaryCloudName,
        out IResourceBuilder<ParameterResource> cloudinaryApiKey,
        out IResourceBuilder<ParameterResource> cloudinaryApiSecret)
    {
        cloudinaryCloudName = builder.AddParameter("cloudinaryCloudName");
        cloudinaryApiKey = builder.AddParameter("cloudinaryApiKey");
        cloudinaryApiSecret = builder.AddParameter("cloudinaryApiSecret", true);

        return builder;
    }

    private static IDistributedApplicationBuilder AddEmailParameters(
        this IDistributedApplicationBuilder builder,
        out IResourceBuilder<ParameterResource> emailAcsEndpoint,
        out IResourceBuilder<ParameterResource> emailFrontendBaseUrl,
        out IResourceBuilder<ParameterResource> emailSenderAddress)
    {
        emailAcsEndpoint = builder.AddParameter("emailAcsEndpoint");
        emailFrontendBaseUrl = builder.AddParameter("emailFrontendBaseUrl");
        emailSenderAddress = builder.AddParameter("emailSenderAddress", true);

        return builder;
    }
}
