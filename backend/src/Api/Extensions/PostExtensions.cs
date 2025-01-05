using RateMyPet.Core;
using RateMyPet.Infrastructure;

namespace RateMyPet.Api.Extensions;

public static class PostExtensions
{
    public static Uri? GetImageUrl(this Post post, HttpRequest? request)
    {
        if (request is null)
        {
            return null;
        }

        var scheme = request.Scheme;
        var host = request.Host;
        var defaultPort = request.IsHttps ? 443 : 80;
        var port = host.Port ?? defaultPort;
        var path = $"/{BlobContainerNames.Images}/{post.Id}";

        return new UriBuilder(scheme, host.Host, port, path)
            .Uri;
    }
}
